import cytoscape from "cytoscape";
import * as _ from "lodash";

const debouncedFunctions: {[key: string]:  _.DebouncedFunc<(id: string, x: number, y: number) => void>} = {}
function invokeUpdatePosition(id: string, x: number, y: number) {
    window.dotnet.invokeMethodAsync('UpdateNodePositionById', id, x, y);
}

export async function nodePositionChange(e: cytoscape.EventObject) {
    const node: cytoscape.NodeSingular = e.target;
    const id = node.data('id');
    const position = node.position();

    const prev = debouncedFunctions[id];
    if(prev) {
        prev.cancel();
    }

    debouncedFunctions[id] = _.debounce(invokeUpdatePosition, 600);
    debouncedFunctions[id](id, position.x, position.y);
}
