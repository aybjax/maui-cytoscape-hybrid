import cytoscape, { type NodeDefinition } from 'cytoscape';
import dagre from 'cytoscape-dagre';
import cxtmenu from 'cytoscape-cxtmenu';
import cytoscapePopper from 'cytoscape-popper';
import { createPopper } from '@popperjs/core';
interface Dotnet {
    invokeMethodAsync(fnx: 'AddMicrotopic'|'DeleteMicrotopicById'|'AddEdgeBySourceTarget'|'DeleteEdgeById',
                      ...params: string[]): Promise<any>;
    dispose():void;
}
declare global {
    interface Window {
        createCytoscape: (a: string)=>Promise<cytoscape.Core>;
        cy: cytoscape.Core;
        dotnet: Dotnet;
        initDotnet: (dotnet: Dotnet) => void;
        disposeDotnet: () => void;
        addMicrotopic: (id: string, name: string, color: string) => void;
        deleteMicrotopic: (nodeId: string, edgeIds: string[]) => void;
        addEdge: (id: string, source: string, target: string) => void;
        deleteEdge: (id: string) => void;
    }
}
window.initDotnet = (dotnet:  Dotnet) => {
    window.dotnet = dotnet;
}
window.disposeDotnet = () => {
    window.dotnet?.dispose();
}
window.addMicrotopic = (id: string, name: string, color: string) => {
    window.cy.one('tap', (e) => {
        window.cy.add({
            data: {
                id: id,
                name: name,
                bg:  color,
                group: 'microtopics',
                selectable: true,
            },
            position: e.position,
        } as NodeDefinition);

        window.cy.$(`#${id}`).on('dbltap', nodeDbTap);
        window.cy.$(`#${id}`).on('mouseover', nodeMouseOver);
    })
}
window.deleteMicrotopic = (nodeId: string, edgeIds: string[]) => {
    edgeIds.forEach(edgeId => {
        window?.cy.$(`#${edgeId}`).remove();
    })
    window?.cy.$(`#${nodeId}`).remove();
}
window.addEdge = (id: string, source: string, target: string) => {
    window.cy.add({
        data: {
            id,
            source: source,
            target: target,
        }
    })

    window.cy.$(`#${id}`).on('dbltap', edgeDbTap)
}
window.deleteEdge = (id: string) => {
    console.log(id)
    console.dir(id)
    console.log(JSON.stringify(id))
    window.cy?.$(`#${id}`).remove();
}

let nodeFirst: cytoscape.NodeDataDefinition |undefined;

cytoscape.use(dagre);
cytoscape.use(cxtmenu);
cytoscape.use( cytoscapePopper(createPopper) );

window.createCytoscape = async function createCytoscape(data: string = '[]'): Promise<cytoscape.Core> {
    if(window.cy) {
        try{
            window.cy?.destroy();
        }catch{}
    }
    window.cy = cytoscape({
        container: document.getElementById('graph'),

        elements: JSON.parse(data),

        layout: {
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
        },
        wheelSensitivity: 0.2,
        motionBlur: true,
        // maxZoom: 100,

        style: [
            {
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
            },
            {
                "selector": "edge",
                "style": {
                    'curve-style': 'bezier',
                    'target-arrow-shape': 'triangle',
                    'width': 5
                }
            },
            {
                "selector": "node:selected",
                "style": {
                    "background-color": "#000",
                },
            }
        ]
    });

    window.cy.edges().on('dbltap', edgeDbTap)
    window.cy.on('tap', bgTap);

    window.cy.nodes().on('dbltap', nodeDbTap)
    window.cy.nodes().on('mouseover', nodeMouseOver)

    window.cy.cxtmenu({
        selector: 'core',
        outsideMenuCancel: 10,
        commands: [
            {
                content: 'add mctp',
                select: async function(){
                    await window.dotnet.invokeMethodAsync('AddMicrotopic')
                },
            },
        ]
    })

    window.cy.cxtmenu({
        selector: 'node',
        outsideMenuCancel: 10,
        commands: [
            {
                content: 'remove mctp',
                select: async function(node){
                    await window.dotnet.invokeMethodAsync('DeleteMicrotopicById', node.data('id'));
                }
            },
        ]
    })

    window.cy.forceRender()

    return window.cy;
}

async function nodeMouseOver(e: cytoscape.EventObject) {
    const node = e.target;
    let popper = node.popper({
        content: () => {
            let div = document.createElement('div');

            div.innerHTML = node.data('parent_name') ?? 'no content';
            div.style.backgroundColor = 'white'
            div.style.zIndex = '1000';
            div.style.padding = '10px'
            div.style.border = 'solid black 2px'

            document.body.appendChild(div);
            setTimeout(() => {
                document.body.removeChild(div);
            }, 500);

            return div;
        },
        popper: {} // my popper options here
    });

    let update = () => {
        //@ts-ignore
        popper.update();
    };

    node.on('position', update);
    node.on('mouseout', () => {
    });

    window.cy?.on('pan zoom resize', update);
}

function bgTap(e: cytoscape.EventObject){
    console.dir(e.target)
    if( e.target !== window.cy ) return;
    nodeFirst = undefined;
}
async function edgeDbTap(e: cytoscape.EventObject){
    window.dotnet.invokeMethodAsync('DeleteEdgeById', e.target.data('id'))
}
async function nodeDbTap(e: cytoscape.EventObject) {
    if (!nodeFirst) {
        nodeFirst = e.target
        return
    }

    const existingNode = nodeFirst
    nodeFirst = undefined

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