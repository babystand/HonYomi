'use strict';
import * as ac from './AudioControl';
require("./styles.scss");

var Elm = require('./Main');
var app = Elm.Main.fullscreen();

const audio = document.getElementById('audio');
const source = document.getElementById('audio-source');
audio.onended = () => app.ports.onEnded.send(null);
audio.ontimeupdate = () => app.ports.audioProgress.send(audio.currentTime);
audio.ondurationchange = () => app.ports.durationChange.send(audio.duration);
audio.onplay = () => app.ports.onPlayed.send(null);
audio.onpause = () => app.ports.onPaused.send(null);
function setAudioSource (url){
    source.src = url;
    audio.load();

  app.ports.audioLoaded.send(null);
  let progress = document.getElementById('progress-bar');
  if(progress !== null && progress !== undefined){
    progress.onclick = (e) => {
      let pPos = progress.offsetLeft;
      let cPos = ((e.clientX - progress.getBoundingClientRect().left)/ progress.clientWidth);
      app.ports.onScrub.send(cPos);
    };
  }
}


app.ports.setAudioSource.subscribe(url => setAudioSource(url));
app.ports.playAudio.subscribe(() => ac.play());
app.ports.pauseAudio.subscribe(() => ac.pause());
app.ports.setCurrentTime.subscribe(f => {ac.setCurrentTime(f); app.ports.onTimeSet.send(null);});
