FROM x11docker/xfce
COPY mtg.x86_64 /usr/games/mtg
COPY ["mtg.pck", "/usr/games/mtg.pck"]
RUN export PATH=$PATH:/usr/games/ 
ENV PATH="/usr/games/:${PATH}"
RUN chmod +x /usr/games/mtg
