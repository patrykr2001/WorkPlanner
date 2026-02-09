#!/bin/bash
#
# WorkPlanner - Uruchamia API i Client jednocześnie
# Użycie: ./start.sh [--no-build] [--no-launch]
#

set -e

# Kolory do logowania
INFO='\033[0;36m'
SUCCESS='\033[0;32m'
ERROR='\033[0;31m'
NC='\033[0m' # No Color

# Funkcje logowania
log_info() {
    echo -e "${INFO}[INFO]${NC} $1"
}

log_success() {
    echo -e "${SUCCESS}[OK]${NC} $1"
}

log_error() {
    echo -e "${ERROR}[ERROR]${NC} $1"
}

# Parsowanie argumentów
NO_BUILD=false
NO_LAUNCH=false

while [[ $# -gt 0 ]]; do
    case $1 in
        --no-build)
            NO_BUILD=true
            shift
            ;;
        --no-launch)
            NO_LAUNCH=true
            shift
            ;;
        *)
            echo "Nieznany argument: $1"
            echo "Użycie: $0 [--no-build] [--no-launch]"
            exit 1
            ;;
    esac
done

# Ścieżka do katalogu solucji
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

log_info "WorkPlanner - Uruchamianie API i Client..."
log_info "Katalog: $SCRIPT_DIR"

# Budowanie (opcjonalnie)
if [ "$NO_BUILD" = false ]; then
    log_info "Budowanie projektów..."
    dotnet build WorkPlanner.Api/WorkPlanner.Api.csproj --configuration Debug > /dev/null 2>&1
    dotnet build WorkPlanner.Client/WorkPlanner.Client.csproj --configuration Debug > /dev/null 2>&1
    log_success "Budowanie zakończone"
fi

# Funkcja do czyszczenia procesów po zakończeniu
cleanup() {
    log_info "Zatrzymywanie serwisów..."
    
    if [ -n "$API_PID" ] && kill -0 "$API_PID" 2>/dev/null; then
        kill -TERM "$API_PID" 2>/dev/null || true
        wait "$API_PID" 2>/dev/null || true
        log_success "API zatrzymane"
    fi
    
    if [ -n "$CLIENT_PID" ] && kill -0 "$CLIENT_PID" 2>/dev/null; then
        kill -TERM "$CLIENT_PID" 2>/dev/null || true
        wait "$CLIENT_PID" 2>/dev/null || true
        log_success "Client zatrzymany"
    fi
    
    log_success "WorkPlanner zatrzymany."
    exit 0
}

# Ustaw trap dla sygnałów (Ctrl+C, itp.)
trap cleanup SIGINT SIGTERM EXIT

# Przygotowanie argumentów --no-build
BUILD_ARG=""
if [ "$NO_BUILD" = true ]; then
    BUILD_ARG="--no-build"
fi

# Przygotowanie argumentów --no-launch
LAUNCH_ARG=""
if [ "$NO_LAUNCH" = true ]; then
    LAUNCH_ARG="--no-launch-profile"
fi

# Uruchomienie API
log_info "Uruchamianie API (https://localhost:7191)..."
dotnet run --project WorkPlanner.Api/WorkPlanner.Api.csproj --launch-profile https $BUILD_ARG > /dev/null 2>&1 &
API_PID=$!

# Poczekaj chwilę na uruchomienie API
sleep 3

# Sprawdź czy API się uruchomiło
if ! kill -0 "$API_PID" 2>/dev/null; then
    log_error "API nie uruchomiło się poprawnie!"
    exit 1
fi

log_success "API uruchomione (PID: $API_PID)"

# Uruchomienie Client
log_info "Uruchamianie Client (https://localhost:7127)..."
dotnet run --project WorkPlanner.Client/WorkPlanner.Client.csproj --launch-profile https $BUILD_ARG $LAUNCH_ARG > /dev/null 2>&1 &
CLIENT_PID=$!

# Poczekaj chwilę na uruchomienie Client
sleep 3

# Sprawdź czy Client się uruchomił
if ! kill -0 "$CLIENT_PID" 2>/dev/null; then
    log_error "Client nie uruchomił się poprawnie!"
    kill -TERM "$API_PID" 2>/dev/null || true
    exit 1
fi

log_success "Client uruchomiony (PID: $CLIENT_PID)"
echo ""
echo -e "${SUCCESS}===============================================${NC}"
echo -e "${SUCCESS}WorkPlanner jest uruchomiony!${NC}"
echo -e "${SUCCESS}API:    https://localhost:7191${NC}"
echo -e "${SUCCESS}Client: https://localhost:7127${NC}"
echo -e "${SUCCESS}===============================================${NC}"
echo ""
echo -e "\033[0;33mNaciśnij CTRL+C, aby zatrzymać oba serwisy...\033[0m"

# Czekaj na zakończenie procesów (będą działać w tle)
wait
