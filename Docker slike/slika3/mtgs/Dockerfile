FROM x11docker/xfce
RUN apt-get update && apt-get install ca-certificates wget -y
RUN wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN apt-get update && apt-get install -y aspnetcore-runtime-8.0
COPY MTGS.x86_64 /usr/games/mtgs
RUN export PATH=$PATH:/usr/games/ 
ENV PATH="/usr/games/:${PATH}"
RUN chmod +x /usr/games/mtgs
