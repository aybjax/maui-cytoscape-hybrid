import {Stylesheet} from "cytoscape";

export const edge_style: Stylesheet = {
    "selector": "edge",
    "style": {
        'curve-style': 'bezier',
        'target-arrow-shape': 'triangle',
        'width': 5
    }
}