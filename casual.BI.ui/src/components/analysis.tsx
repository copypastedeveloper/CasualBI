import React, { useEffect, useState } from "react";
import ai from "../api/AIQueryService";
import tablemark from 'tablemark';
import ReactMarkdown from 'react-markdown'

interface AnalysisProps {
    question: string;
    data: Array<any>
}

const Analysis : React.FC<AnalysisProps> = ({question,data}) => {

    const [analysis,setAnalysis] = useState('')
    useEffect(() => {
        if (!(data?.length) || !question) return;

        const table = tablemark(data);

        ai.analyze(table,question, (chunk) => {
            setAnalysis(a => a + chunk);
        });
    },[question,data])

    return (
        <>
            <div>
                <ReactMarkdown>
                    {analysis}
                </ReactMarkdown>
            </div>
        </>
    )
}

export default Analysis