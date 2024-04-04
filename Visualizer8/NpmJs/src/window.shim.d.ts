import cytoscape from "cytoscape";


declare global {
    interface Window {
        cy: cytoscape.Core;
        createCytoscape: (a: string|undefined = void 0)=>Promise<cytoscape.Core>;
        //
        nodeFirst: cytoscape.NodeDataDefinition |undefined
        //
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