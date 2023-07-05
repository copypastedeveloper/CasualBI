import {useEffect, useState} from 'react';
import {AsyncDuckDB, AsyncDuckDBConnection} from "@duckdb/duckdb-wasm";
import {getDuckDb} from "./getDuckDb";

type DuckDbHelpers = {
    createParquetView(name: string, uri: string): Promise<void>;
    tableExists: ITableExists,
    buildTableFromBuffer: IBuildTableFromBuffer,
    buildTableFromText: IBuildTableFromText,
    query: IQueryTable
}

type ITableExists = (name : string) => Promise<boolean>;
type IBuildTableFromBuffer = (name: string, data: ArrayBuffer) => Promise<void>;
type IBuildTableFromText = (name: string, data: string) => Promise<void>;
type IQueryTable = (name: string) => Promise<object[]>;

const useDuckDb = () : [AsyncDuckDB|null, AsyncDuckDBConnection|null, DuckDbHelpers] => {

    const [db, setDb] = useState<AsyncDuckDB|null>(null);
    const [conn, setConn] = useState<AsyncDuckDBConnection | null>(null);

    useEffect(() => {
        const createDbAndConnection = async () => {
            const database = await getDuckDb();
            setDb(database);
            const newConnection = await database.connect();
            setConn(newConnection);
        };
        
        createDbAndConnection()
            .catch(console.error);
    },[]);

    const buildTableFromBuffer : IBuildTableFromBuffer = async (name: string, data: ArrayBuffer) : Promise<void> => {
        const tableExists = await help.tableExists(name.replace(' ', '_'));
        if (tableExists) return;

        await db?.registerFileBuffer(`${name.replace(' ', '_')}.csv`, new Uint8Array(data));
        await conn?.query(`CREATE TABLE "${name.replace(' ', '_')}" AS SELECT * FROM read_csv_auto('${name.replace(' ', '_')}.csv', header=true)`);
    };

    const buildTableFromText : IBuildTableFromText = async (name: string, data: string) : Promise<void> => {
        const tableExists = await help.tableExists(name.replace(' ', '_'));
        if (tableExists) return;

        await db?.registerFileText(`${name.replace(' ', '_')}.csv`, data);
        await conn?.query(`CREATE TABLE "${name.replace(' ', '_')}" AS SELECT * FROM read_csv_auto('${name.replace(' ', '_')}.csv', header=true)`);
    };

    const createParquetView : (name: string, uri: string) => Promise<void> = async (name: string, uri: string) : Promise<void> => {
        const tableExists = await help.tableExists(name.replace(' ', '_'));
        if (tableExists) return;

        await conn?.query(`CREATE table "${name.replace(' ', '_')}" AS SELECT * FROM '${uri}';`);
    };

    const tableExists : ITableExists = async (name : string) : Promise<boolean> => {
        let queryResult = (await conn?.query(`select exists(select * from information_schema.tables where table_name = '${name.replace(' ', '_')}') as ItExists`)).toArray();
        return queryResult[0].ItExists as boolean;
    };

    const query = async (q: string)  : Promise<object[]> => {
        if (!conn) throw new Error("connection is not initialized");
        const result = await conn.query(q);
        return result.toArray();
    };

    const help = {
        tableExists: tableExists,
        buildTableFromBuffer: buildTableFromBuffer,
        buildTableFromText: buildTableFromText,
        createParquetView: createParquetView,
        query: query,
    };

    return [db, conn, help];
};

export default useDuckDb;
