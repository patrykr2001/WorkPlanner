# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files
COPY WorkPlanner.slnx .
COPY WorkPlanner.Client/WorkPlanner.Client.csproj WorkPlanner.Client/

# Restore dependencies
RUN dotnet restore WorkPlanner.Client/WorkPlanner.Client.csproj

# Copy source code
COPY WorkPlanner.Client/ WorkPlanner.Client/

# Build and publish
WORKDIR /src/WorkPlanner.Client
RUN dotnet publish -c Release -o /app/publish

# Runtime stage with nginx
FROM nginx:alpine AS runtime

# Copy custom nginx configuration
COPY WorkPlanner.Client/nginx.conf /etc/nginx/conf.d/default.conf

# Copy published Blazor WASM app
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Create symlinks for hashed Blazor files (remove hash from filename for clean URLs)
RUN cd /usr/share/nginx/html/_framework && \
    for f in blazor.webassembly.*.js; do \
        [ -f "$f" ] && ln -sf "$f" blazor.webassembly.js; \
    done && \
    for f in blazor.webassembly.*.js.br; do \
        [ -f "$f" ] && ln -sf "$f" blazor.webassembly.js.br; \
    done && \
    for f in blazor.webassembly.*.js.gz; do \
        [ -f "$f" ] && ln -sf "$f" blazor.webassembly.js.gz; \
    done

# Create config file for API URL (can be overridden at runtime)
RUN echo '{"ApiBaseUrl":"/api"}' > /usr/share/nginx/html/appsettings.json

# Expose port
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
