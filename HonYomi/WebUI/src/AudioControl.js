export function play(){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    audio.play();
  }
}
export function pause(){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    audio.pause();
  }
}
export function setCurrentTime(f){
  let audio = document.getElementById('audio');
  if(audio !== null && audio !== undefined){
    audio.currentTime = f;
  }
}
