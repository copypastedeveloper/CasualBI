import React, {useEffect, useState} from "react";
// @ts-ignore
import useDuckDb from "./useDuckDb";
// @ts-ignore
import AIQueryService from "../api/AIQueryService";
import {Dataset} from "../api/contracts";

export interface DuckDbStuffProps {
    onDataChanged(data:  object[]) : void;
    datasets: Dataset[];
    question: string;
}
interface ColumnInfo {
    table:string;
    column:string;
    type:string;
}

const DuckDbService : React.FC<DuckDbStuffProps> = ({onDataChanged, datasets, question}) => {
    const [, , db] = useDuckDb();
    const [chatHistory, setChatHistory] = useState<any>()

    const handleQuery = async ( response : any) => {
        return new Promise((resolve, reject) => {
            console.log(response);
            setChatHistory(response.chatHistory);
            db.query(response.query).then(results => {
                if (results.length) return results;
                throw(new Error("This query returned no results"))
            }).then((results: object[]) => {
                onDataChanged(results);
                resolve(results);
            }).catch(reason => reject(reason))
        })
    }

    const loadTable = async (dataset: Dataset) => {
        const exists = await db.tableExists(dataset.name);
        if (exists) return;

        if (dataset.type == 'parquet') 
            await db.createParquetView(dataset.name, dataset.uri);
    };

    const loadTables = async (datasets: Dataset[]) => {
        await Promise.all(
            datasets.map(async (ds) =>
                await loadTable(ds))
        );
    };

    const fixQuery = async (error:string, retryCount:number) => {
        if (retryCount > 5) return;
        AIQueryService.fix(chatHistory, error).then(handleQuery)
        .catch((e) => {
            console.error(e);
            fixQuery(e.message,++retryCount);
        });
    }

    useEffect(() => {
        if (datasets.length === 0) return;
        if (!question) return;

        let chatHistory = Array<any>;
        loadTables(datasets)
            .then(async () => {
                let columns: Array<ColumnInfo> = [];
                let context = '';
                await db.query("select table_name, column_name, data_type from information_schema.columns")
                .then((r:any) => { 
                    r.forEach((c: any) => {
                        columns.push({table: c.table_name, column:c.column_name,type:c.data_type});
                    })
                }).catch(e => console.error(e));

                const map = new Map<string,Array<ColumnInfo>>();
                columns.forEach((c) => {
                    const collection = map.get(c.table);
                    if (!collection) {
                        map.set(c.table, [c]);
                    } else {
                        collection.push(c);
                    }
                });
                
                map.forEach((columns,table) => {
                    context += `"Table: ${table}"\n----------\n`;
                    columns.forEach(c => {
                        context += `"${c.column}" (${c.type}) \n`;
                    });
                    context += '\n\n';
                });
                return context;
            })
            .then(async (tableContext) => {
                AIQueryService.initiateAgent(tableContext, question);
                return await AIQueryService.build(tableContext, question)
            })
            .then(handleQuery)
            .catch((e) => {
                console.error(e);
                fixQuery(e.message,0);
            });
    },[question]);

    return <></>;
};

export default DuckDbService;
