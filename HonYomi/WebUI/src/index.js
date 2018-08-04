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
    audio.load();
    audio.onended = () => app.ports.onEnded.send(null);
    audio.ontimeupdate = () => app.ports.audioProgress.send(audio.currentTime);
    audio.ondurationchange = () => app.ports.durationChange.send(audio.duration);
    audio.onplay = () => app.ports.onPlayed.send(null);
    audio.onpause = () => app.ports.onPaused.send(null);
    audio.play();
  }
  let progress = document.getElementById('progress-bar');
  if(progress !== null && progress !== undefined){
    progress.onclick = (e) => {
      console.log(e);
      let pPos = progress.offsetLeft;
      let cPos = ((e.clientX - progress.getBoundingClientRect().left)/ progress.clientWidth);
      console.log("offset left: "+progress.offsetLeft);
      console.log("clientx: " + e.clientX);
      console.log("cwidth: " +progress.clientWidth);
      console.log("cpos: " +cPos);

      app.ports.onScrub.send(cPos);
    };
  }
}


app.ports.loadAudioSource.subscribe(() => loadAudioSource());
app.ports.playAudio.subscribe(() => ac.play());
app.ports.pauseAudio.subscribe(() => ac.pause());
app.ports.setCurrentTime.subscribe(f => ac.setCurrentTime(f));
