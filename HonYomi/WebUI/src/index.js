'use strict';
import * as ac from './AudioControl';
require("./styles.scss");

var Elm = require('./Main');
var app = Elm.Main.fullscreen();


function loadAudioSource (){
  let audio = document.getElementById('audio');
  console.log("loading source");
  if(audio !== null && audio !== undefined){
    audio.load();
    audio.onended = () => app.ports.onEnded.send(null);
    audio.ontimeupdate = () => app.ports.audioProgress.send(audio.currentTime);
    audio.ondurationchange = () => app.ports.durationChange.send(audio.duration);
    audio.onplay = () => app.ports.onPlayed.send(null);
    audio.onpause = () => app.ports.onPaused.send(null);
  }
}


app.ports.loadAudioSource.subscribe(() => loadAudioSource());
app.ports.playAudio.subscribe(() => ac.play());
app.ports.pauseAudio.subscribe(() => ac.pause());
