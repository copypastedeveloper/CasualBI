import {HttpService} from "./HttpService";

const httpService = HttpService.create();
class AIQueryService {
    build(tableContext: string, question: string) : Promise<any> {
        return new Promise((resolve, reject) => {
            httpService
                .fetchAsJson(`/query/build`,{method:"post"},{tableContext,question})
                .then(data => resolve(data))
                .catch(err => reject(err));
        });
    }
    initiateAgent(tableContext: string, question: string) : Promise<any> {
        return new Promise((resolve, reject) => {
            httpService
                .fetchAsJson(`/api/agent/initiate`,{method:"post"},{tableContext,question})
                .then(data => resolve(data))
                .catch(err => reject(err));
        });
    }
    fix(chatHistory: string, error: string) : Promise<any>{
        return new Promise((resolve, reject) => {
            httpService
                .fetchAsJson(`/query/fix`,{method:"post"},{chatHistory,error})
                .then(data => resolve(data))
                .catch(err => reject(err));
        });
    }
    analyze(tableContext: string,question: string,callbackPerChunk: (chunk: string) => any) : void {
        httpService
            .fetch(`/DataAnalysis`,{method:"post"},{tableContext,question})
            .then(async response => {
                const reader = response.body?.getReader();
                if (!reader) {
                    throw new Error('Failed to read response');
                }
                const decoder = new TextDecoder();

                //eslint-disable-next-line
                while (true) {
                    const { done, value } = await reader.read();
                    if (done) break;
                    if (!value) continue;
                    var item = decoder.decode(value);
                    await callbackPerChunk(item);
                }
                reader.releaseLock();
            });
    }
}


let svc = new AIQueryService();
export default svc