import cytoscape, { type NodeDefinition } from 'cytoscape';
import dagre from 'cytoscape-dagre';
import cxtmenu from 'cytoscape-cxtmenu';
import cytoscapePopper from 'cytoscape-popper';
import { createPopper } from '@popperjs/core';
import * as _ from 'lodash';

interface Dotnet {
    invokeMethodAsync(fnx: 'AddEdgeBySourceTarget', sourceId: string, targetId: string): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateMicrotopicById'|'DeleteMicrotopicById'|'DeleteEdgeById', id: string): Promise<void>;
    invokeMethodAsync(fnx: 'AddMicrotopic'): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateNodePositionById'|'InitiateNodePositionById', id: string, x: number, y: number): Promise<void>;
    dispose():void;
}

declare global {
    interface Window {
        createCytoscape: (a: string)=>Promise<cytoscape.Core>;
        cy: cytoscape.Core;
        dotnet: Dotnet;
        initDotnet: (dotnet: Dotnet) => void;
        disposeDotnet: () => void;
        addMicrotopic: (id: string, name: string, parentName: string, color: string) => void;
        addMicrotopicWithPosition: (id: string, name: string, parentName: string, color: string, x: number, y: number) => void;
        updateMicrotopic: (id: string, name: string, parentName: string, color: string) => void;
        deleteMicrotopic: (nodeId: string, edgeIds: string[]) => void;
        addEdge: (id: string, source: string, target: string) => void;
        deleteEdge: (id: string) => void;
        updatePosition: (id: string, x: number, y: number) => void;
    }
}
window.initDotnet = (dotnet:  Dotnet) => {
    window.dotnet = dotnet;
}
window.disposeDotnet = () => {
    window.dotnet?.dispose();
}
window.addMicrotopic = (id: string, name: string, parentName: string, color: string) => {
    window.cy.one('tap', (e) => {
        window.cy.add({
            data: {
                id: id,
                name: name,
                parent_name: parentName,
                bg:  color,
                group: 'microtopics',
                selectable: true,
            },
            position: e.position,
        } as NodeDefinition);

        window.cy.$(`#${id}`).on('dbltap', nodeDbTap);
        window.cy.$(`#${id}`).on('mouseover', nodeMouseOver);
        window.cy.$(`#${id}`).on('position', nodePositionChange);
    })
}
window.addMicrotopicWithPosition = (id: string, name: string, parentName: string, color: string, x: number, y: number) => {
    window.cy.add({
        data: {
            id: id,
            name: name,
            parent_name: parentName,
            bg:  color,
            group: 'microtopics',
            selectable: true,
        },
        position: {x, y},
    } as NodeDefinition);

    window.cy.$(`#${id}`).on('dbltap', nodeDbTap);
    window.cy.$(`#${id}`).on('mouseover', nodeMouseOver);
    window.cy.$(`#${id}`).on('position', nodePositionChange)
}
window.updateMicrotopic = (id: string, name: string, parentName: string, color: string) => {
    window.cy.$(`#${id}`)
        .data('name', name)
        .data('parent_name', parentName)
        .data('bg', color);
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

window.updatePosition = (id: string, x: number, y: number) => {
    window.cy.$(`#${id}`).off('position');
    window.cy?.$(`#${id}`).position({
        x,
        y,
    });
    window.cy.$(`#${id}`).on('position', nodePositionChange);
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
    let parsedData: cytoscape.ElementDefinition[] = JSON.parse(data)
    window.cy = cytoscape({
        container: document.getElementById('graph'),

        elements: parsedData,

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
    
    window.cy.forceRender();

    debugger
    // window.cy.forceRender()
    if(parsedData.some(el => !!el.position))
    {
        debugger
        parsedData = JSON.parse(data)
        parsedData.forEach(el => {
            if(el.position?.x !== null && el.position?.x !== void 0 && el.position?.y !== null && el.position?.y !== void 0)
            {
                console.log(el.position)
                window.cy.$(`#${el.data.id}`)
                    .position({
                        x: el.position!.x,
                        y: el.position!.y,
                    })
            }
        })
    }
    
    window.cy.nodes().forEach(node => {
        const position = node.position();
        window.dotnet.invokeMethodAsync('InitiateNodePositionById', node.data('id'), position.x, position.y)
    })

    window.cy.edges().on('dbltap', edgeDbTap)
    window.cy.on('tap', bgTap);

    window.cy.nodes().on('dbltap', nodeDbTap)
    window.cy.nodes().on('mouseover', nodeMouseOver)
    window.cy.nodes().on('position', nodePositionChange)

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
                content: 'delete',
                select: async function(node){
                    await window.dotnet.invokeMethodAsync('DeleteMicrotopicById', node.data('id'));
                }
            },
            {
                content: 'update',
                select: async function(node){
                    await window.dotnet.invokeMethodAsync('UpdateMicrotopicById', node.data('id'));
                }
            },
        ]
    })
    
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

function invokeUpdatePosition(id: string, x: number, y: number) {
    window.dotnet.invokeMethodAsync('UpdateNodePositionById', id, x, y);
}

const debouncedFunctions: {[key: string]:  _.DebouncedFunc<(id: string, x: number, y: number) => void>} = {}
async function nodePositionChange(e: cytoscape.EventObject) {
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