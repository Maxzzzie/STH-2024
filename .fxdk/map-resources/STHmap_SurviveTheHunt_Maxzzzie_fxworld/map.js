// AUTOMATICALLY GENERATED FILE
// ANY CHANGES TO THIS FILE WILL BE OVERWRITTEN
//
// Compiled at 2024-09-29T21.29.41
setTimeout(EnableEditorRuntime, 0);
on('onResourceStop', (name) => { if (name === GetCurrentResourceName()) DisableEditorRuntime(); });
// Map patches
function applyPatch(md,_e,u){const e=GetEntityIndexFromMapdata(md,_e);if(e===-1)return console.error('Failed to get entity index from mapdata',{mapdataHash:_md,mapdataIndex:md,entityHash:_e});UpdateMapdataEntity(md,e,u)}