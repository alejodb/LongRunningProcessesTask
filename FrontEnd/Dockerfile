FROM node:lts-jod AS build-react
WORKDIR /app

COPY FrontEnd/long-running-processes-client/package.json ./
RUN npm install

COPY FrontEnd/long-running-processes-client/. .
ARG VITE_API_URL=/api
RUN npm run build

FROM nginx:alpine
COPY --from=build-react /app/dist /usr/share/nginx/html

COPY FrontEnd/nginx.conf /etc/nginx/conf.d/default.conf
COPY FrontEnd/.htpasswd /etc/nginx/.htpasswd

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]