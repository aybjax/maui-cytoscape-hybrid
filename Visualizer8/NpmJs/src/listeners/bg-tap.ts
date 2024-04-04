import cytoscape from "cytoscape";

export function bgTap(e: cytoscape.EventObject){
    console.dir(e.target)
    if( e.target !== window.cy ) return;
    window.nodeFirst = undefined;
}