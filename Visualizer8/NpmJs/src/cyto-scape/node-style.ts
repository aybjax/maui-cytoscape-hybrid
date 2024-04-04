import {Stylesheet} from "cytoscape";

export const node_style: Stylesheet = {
    "selector": "node",
    "style": {
        "label": "data(name)",
        "background-color": "data(bg)",
        'font-size': '18px',
        'text-wrap': 'wrap',
        'border-width': '0px',
        "text-valign": "top",
        "text-halign": "center",
        "width": 100,
        "height": 100,
        "text-background-color": "rgb(255,255,255)",
        "text-background-opacity": 1,
        "text-opacity": 1,
        "opacity": 0.7,
        'text-margin-x': 0,
        'text-margin-y': -15,
        "z-index": 100,
        "text-max-width": "200",
    }
}