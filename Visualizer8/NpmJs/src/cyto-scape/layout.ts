import {LayoutOptions} from "cytoscape";

export const layout: LayoutOptions = {
    //@ts-ignore
    name: 'dagre',
    // @ts-ignore
    animate: false,
    nodeDimensionsIncludeLabels: true,
    //@ts-ignore
    idealEdgeLength: 100,
    // Extra spacing between components in non-compound graphs
    componentSpacing: 40,

    // Node repulsion (non overlapping) multiplier
    //@ts-ignore
    nodeRepulsion: function( node ){ return 2048; },

    // Node repulsion (overlapping) multiplier
    nodeOverlap: 4,

    // Divisor to compute edge forces
    //@ts-ignore
    edgeElasticity: function( edge ){ return 100; },
}