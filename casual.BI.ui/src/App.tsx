import { Container, Row, Col, Navbar } from "react-bootstrap";
import Analyser from "./pages/analyzer";


function App() {
  return (
    <>
    <Container fluid>
      <Row>
        <Col xs={2} id="sidebar-wrapper">
          {/* <LeftNav /> */}
        </Col>
        <Col xs={10} id="page-content-wrapper">
          <Analyser></Analyser>
        </Col>
      </Row>
    </Container>
    </>
  )
}

export default App
