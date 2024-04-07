interface Dotnet {
    invokeMethodAsync(fnx: 'AddEdgeBySourceTarget', sourceId: string, targetId: string): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateMicrotopicById'|'DeleteMicrotopicById'|'DeleteEdgeById'|'AddUnit', id: string): Promise<void>;
    invokeMethodAsync(fnx: 'AddMicrotopic'): Promise<void>;
    invokeMethodAsync(fnx: 'UpdateNodePositionById'|'InitiateNodePositionById', id: string, x: number, y: number): Promise<void>;
    invokeMethodAsync(fnx: 'addUnit', unitId: string, unitName: string, microtopicIds: string[]): Promise<void>;
    invokeMethodAsync(fnx: 'DeleteUnit', unitId: string): Promise<void>;
    dispose():void;
}