// import settings from "../../configs/appSettings";
// import { CurrentUser } from "../CurrentUser";

export class HttpService {
    config: any;
    constructor(config: { baseURL: any; }) {
        this.config = config;
    }

    static create({baseUrl} = {baseUrl:undefined}) {
        return new HttpService({
            baseURL: baseUrl ?? 'https://localhost:7231',
        });
    }

    public fetch(url: string,options : {},body:{}) {
        return fetch(this.config.baseURL + url,{
            body:JSON.stringify(body),
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${1234}`,              },...options
        });
    }
    public fetchAsJson(url: string,options : {},body:{}) {
        return fetch(this.config.baseURL + url,{
            body:JSON.stringify(body),
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${1234}`,              },...options
        })
        .then(async result => await result.json());
    }
}
