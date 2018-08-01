'use strict';

require("./styles.scss");

var Elm = require('./Main');
var app = Elm.Main.fullscreen();

function loadAudioSource (){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    audio.load();
  }
}
function audioProgress(){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    app.ports.audioProgress.send(audio.currentTime);
  }
}

app.ports.loadAudioSource.subscribe(() => loadAudioSource());
app.ports.getAudioProgress.subscribe(() => audioProgress());
