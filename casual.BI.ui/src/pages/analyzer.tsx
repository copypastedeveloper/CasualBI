import React, { useEffect, useRef, useState } from "react";
import Question from "../components/question";
import {gridReportDefaults} from '../components/grid';
import PivotGrid from '../components/grid'
import Analysis from '../components/analysis'
import DuckDbService from "../duckDb/DuckDbService";
import { Dataset } from "../api/contracts";
import { Container, Row, Col, Navbar } from "react-bootstrap";

const Analyser : React.FC = () => {
    const datasets: Array<Dataset> = [{
        id:'1',
        name:"line items",
        uri: "https://shell.duckdb.org/data/tpch/0_01/parquet/lineitem.parquet",
        type:'parquet',
        dateCreated: new Date(),
        dateModified: new Date(),
        description:''
      },{
        id:'2',
        name:"customers",
        uri: "https://shell.duckdb.org/data/tpch/0_01/parquet/customer.parquet",
        type:'parquet',
        dateCreated: new Date(),
        dateModified: new Date(),
        description:''
      },{
        id:'3',
        name:"orders",
        uri: "https://shell.duckdb.org/data/tpch/0_01/parquet/orders.parquet",
        type:'parquet',
        dateCreated: new Date(),
        dateModified: new Date(),
        description:''
      }]

    const [question, setQuestion] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [data,setData] = useState<Array<any>>([]);
    const gridRef = useRef(null);
    
    const askQuestion: (question:string) => void = (question:string) => {
        setQuestion(question);
        setLoading(true);
    };

    useEffect(() => {
        if (loading) {
        setData([]);
        return;
        }
    },[loading])

    const loadDataInGrid : (data: Array<any>) => void = (data:Array<any>) => {
        
        if (!gridRef.current) return;

        const newReport = {...gridReportDefaults};
        //@ts-ignore
        newReport.dataSource.data = data;
        //@ts-ignore
        gridRef.current.webdatarocks.setReport(newReport);

        setData(data);
        setLoading(false);
    };
    

    return (
        <>
            <Container>
                <Row>
                    <Col xs={3}>
                        <Question onSubmit={askQuestion} loading={loading}></Question>
                    </Col>
                    <Col xs={9}>
                        <PivotGrid innerRef={gridRef} data={data}></PivotGrid>
                    </Col>
                </Row>
                <Row>
                    <Analysis question={question} data={data}></Analysis>
                </Row>
            </Container>
            <DuckDbService datasets={datasets} onDataChanged={loadDataInGrid} question={question} ></DuckDbService>
        </>
    );
}

export default Analyser;