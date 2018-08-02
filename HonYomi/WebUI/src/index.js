'use strict';

require("./styles.scss");

var Elm = require('./Main');
var app = Elm.Main.fullscreen();


function loadAudioSource (){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    audio.load();
    audio.onended = () => app.ports.onEnded.send(null);;
    audio.ontimeupdate = () => app.ports.audioProgress.send(audio.currentTime);
    audio.durationchange = () => app.ports.durationChange.send(audio.duration);
  }
}


app.ports.loadAudioSource.subscribe(() => loadAudioSource());
