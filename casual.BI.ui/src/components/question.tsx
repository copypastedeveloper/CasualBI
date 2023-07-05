import React, { useState } from "react";
import { Button, Col, Row } from "react-bootstrap";

interface QuestionProps {
    onSubmit:(question:string) => void,
    loading:boolean
}

const Question : React.FC<QuestionProps> = ({onSubmit,loading}) => {
    const [question,setQuestion] = useState('');

    return (
        <>
            <Row>
                <Col>
                    <label htmlFor="question">Question:</label>
                    <textarea id="question" rows={10} onChange={(e) => setQuestion(e.currentTarget.value)}/>
                </Col>
            </Row>
            <Row>
                <Col>
                    <Button onClick={(() => onSubmit(question))} disabled={loading}>Ask!</Button>
                </Col>
            </Row>
        </>
    );
}

export default Question;