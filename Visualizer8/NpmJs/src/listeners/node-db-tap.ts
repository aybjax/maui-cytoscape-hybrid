import cytoscape from "cytoscape";

export async function nodeDbTap(e: cytoscape.EventObject) {
    if (!window.nodeFirst) {
        window.nodeFirst = e.target
        return
    }

    const existingNode = window.nodeFirst
    window.nodeFirst = undefined

    if(existingNode === e.target) {
        setTimeout(() => {
            window.cy.$(`#${existingNode.data('id')}`).unselect()
        }, 200)
        return
    }

    await window.dotnet.invokeMethodAsync(
        'AddEdgeBySourceTarget',
        existingNode?.data('id'),
        e.target.data('id'),
    )
}