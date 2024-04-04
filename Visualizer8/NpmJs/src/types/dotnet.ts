interface Dotnet {
    invokeMethodAsync(fnx: 'AddEdgeBySourceTarget', sourceId: string, targetId: string): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateMicrotopicById'|'DeleteMicrotopicById'|'DeleteEdgeById', id: string): Promise<void>;
    invokeMethodAsync(fnx: 'AddMicrotopic'): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateNodePositionById'|'InitiateNodePositionById', id: string, x: number, y: number): Promise<void>;
    dispose():void;
}