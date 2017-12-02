FROM nginx:1.13.6
COPY farm_example_app.conf /etc/nginx/conf.d/farm_example_app.conf
COPY nginx.conf /etc/nginx/nginx.conf
RUN [ "mv", "/etc/nginx/conf.d/default.conf", "/etc/nginx/conf.d/default.conf_" ]