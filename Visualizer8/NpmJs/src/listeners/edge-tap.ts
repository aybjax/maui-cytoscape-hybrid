import cytoscape from "cytoscape";

export async function edgeDbTap(e: cytoscape.EventObject){
    window.dotnet.invokeMethodAsync('DeleteEdgeById', e.target.data('id'))
}