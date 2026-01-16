FROM node:24-alpine AS build
WORKDIR /app
COPY ./steganography/package*.json ./
RUN npm ci
COPY ./steganography/ ./
RUN npm run build -- --configuration production

FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/dist/*/browser /usr/share/nginx/html
EXPOSE 8080
ENTRYPOINT ["nginx", "-c", "/etc/nginx/nginx.conf"]
CMD ["-g", "daemon off;"]