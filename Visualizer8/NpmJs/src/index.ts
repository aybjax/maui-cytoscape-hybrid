import cytoscape, {type NodeDefinition} from 'cytoscape';
import dagre from 'cytoscape-dagre';
import cxtmenu from 'cytoscape-cxtmenu';
import cytoscapePopper from 'cytoscape-popper';
import { createPopper } from '@popperjs/core';
import {edgeDbTap} from "./listeners/edge-tap.ts";
import {bgTap} from "./listeners/bg-tap.ts";
import {nodeDbTap} from "./listeners/node-db-tap.ts";
import {nodeMouseOver} from "./listeners/mouse-over.ts";
import {nodePositionChange} from "./listeners/position-change.ts";
import {layout} from "./cyto-scape/layout.ts";
import {node_style} from "./cyto-scape/node-style.ts";
import {edge_style} from "./cyto-scape/edge-style.ts";
import {nodeSelected_style} from "./cyto-scape/node_selected-style.ts";

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

        layout: layout,
        wheelSensitivity: 0.2,
        motionBlur: true,
        // maxZoom: 100,

        style: [
            node_style,
            edge_style,
            nodeSelected_style,
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