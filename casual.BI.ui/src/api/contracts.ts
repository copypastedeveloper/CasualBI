export type Dataset = {
    id: string,
    name: string,
    description: string|undefined,
    uri: string,
    type:string,
    dateCreated : Date | undefined,
    dateModified : Date | undefined,
}
