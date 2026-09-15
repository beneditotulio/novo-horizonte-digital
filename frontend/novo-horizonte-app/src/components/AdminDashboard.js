import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Form, Button, ListGroup, Alert, Tab, Tabs } from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import { ApiService } from '../services/ApiService';

export default function AdminDashboard() {
  const [areas, setAreas] = useState([]);
  const [newArea, setNewArea] = useState({
    name: '',
    dimensions: '',
    adhesionValue: '',
    monthlyInstallment: '',
    paymentPeriodMonths: 24,
    housingStandard: 'Médio'
  });
  const [newLot, setNewLot] = useState({
    name: '',
    areaId: ''
  });
  const [newPlot, setNewPlot] = useState({
    areaId: '',
    lotId: '',
    plotNumber: ''
  });
  const [plotRange, setPlotRange] = useState({
    areaId: '',
    lotId: '',
    startNumber: '',
    endNumber: ''
  });
  const [lots, setLots] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadAreas();
  }, []);

  const loadAreas = async () => {
    try {
      const data = await ApiService.getAreas();
      setAreas(data);
    } catch (error) {
      toast.error('Erro ao carregar áreas');
    }
  };

  const handleCreateArea = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await ApiService.createArea({
        name: newArea.name,
        dimensions: newArea.dimensions,
        adhesionValue: parseFloat(newArea.adhesionValue),
        monthlyInstallment: parseFloat(newArea.monthlyInstallment),
        paymentPeriodMonths: parseInt(newArea.paymentPeriodMonths),
        housingStandard: newArea.housingStandard
      });
      toast.success('Área criada com sucesso!');
      setNewArea({
        name: '',
        dimensions: '',
        adhesionValue: '',
        monthlyInstallment: '',
        paymentPeriodMonths: 24,
        housingStandard: 'Médio'
      });
      loadAreas();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao criar área');
    } finally {
      setLoading(false);
    }
  };

  const handleCreateLot = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await ApiService.createLot({
        name: newLot.name,
        areaId: parseInt(newLot.areaId)
      });
      toast.success('Lote criado com sucesso!');
      setNewLot({ name: '', areaId: '' });
      loadAreas();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao criar lote');
    } finally {
      setLoading(false);
    }
  };

  const handleCreatePlot = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await ApiService.createPlot({
        areaId: parseInt(newPlot.areaId),
        lotId: parseInt(newPlot.lotId),
        plotNumber: newPlot.plotNumber
      });
      toast.success('Terreno criado com sucesso!');
      setNewPlot({ areaId: '', lotId: '', plotNumber: '' });
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao criar terreno');
    } finally {
      setLoading(false);
    }
  };

  const handleCreatePlotRange = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      const response = await ApiService.createPlotsInRange({
        areaId: parseInt(plotRange.areaId),
        lotId: parseInt(plotRange.lotId),
        startNumber: parseInt(plotRange.startNumber),
        endNumber: parseInt(plotRange.endNumber)
      });
      toast.success(response.message);
      setPlotRange({ areaId: '', lotId: '', startNumber: '', endNumber: '' });
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao criar terrenos em intervalo');
    } finally {
      setLoading(false);
    }
  };

  const handleAreaChange = async (areaId) => {
    setNewLot({ ...newLot, areaId });
    setNewPlot({ ...newPlot, areaId });
    setPlotRange({ ...plotRange, areaId });
    try {
      const data = await ApiService.getLotsByArea(parseInt(areaId));
      setLots(data);
    } catch (error) {
      toast.error('Erro ao carregar lotes');
    }
  };

  return (
    <Container className="py-4">
      <h2 className="mb-4">Dashboard do Administrador</h2>

      <Tabs defaultActiveKey="areas" className="mb-4">
        {/* Areas Tab */}
        <Tab eventKey="areas" title="Gestão de Áreas">
          <Row className="mt-4">
            <Col md={6}>
              <Card>
                <Card.Header className="bg-primary text-white">
                  <Card.Title className="mb-0">Criar Nova Área</Card.Title>
                </Card.Header>
                <Card.Body>
                  <Form onSubmit={handleCreateArea}>
                    <Form.Group className="mb-3">
                      <Form.Label>Nome da Área</Form.Label>
                      <Form.Control
                        type="text"
                        value={newArea.name}
                        onChange={(e) => setNewArea({ ...newArea, name: e.target.value })}
                        placeholder="Ex: Terreno Jovem"
                        required
                      />
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Dimensões</Form.Label>
                      <Form.Control
                        type="text"
                        value={newArea.dimensions}
                        onChange={(e) => setNewArea({ ...newArea, dimensions: e.target.value })}
                        placeholder="Ex: 15x30"
                        required
                      />
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Valor de Adesão (MT)</Form.Label>
                      <Form.Control
                        type="number"
                        value={newArea.adhesionValue}
                        onChange={(e) => setNewArea({ ...newArea, adhesionValue: e.target.value })}
                        placeholder="0.00"
                        required
                      />
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Prestação Mensal (MT)</Form.Label>
                      <Form.Control
                        type="number"
                        value={newArea.monthlyInstallment}
                        onChange={(e) => setNewArea({ ...newArea, monthlyInstallment: e.target.value })}
                        placeholder="0.00"
                        required
                      />
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Prazo (meses)</Form.Label>
                      <Form.Select
                        value={newArea.paymentPeriodMonths}
                        onChange={(e) => setNewArea({ ...newArea, paymentPeriodMonths: e.target.value })}
                      >
                        <option value={12}>12 meses</option>
                        <option value={15}>15 meses</option>
                        <option value={24}>24 meses</option>
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Padrão Habitacional</Form.Label>
                      <Form.Select
                        value={newArea.housingStandard}
                        onChange={(e) => setNewArea({ ...newArea, housingStandard: e.target.value })}
                      >
                        <option value="Básico">Básico</option>
                        <option value="Médio">Médio</option>
                        <option value="Alto">Alto</option>
                      </Form.Select>
                    </Form.Group>

                    <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                      {loading ? 'Criando...' : 'Criar Área'}
                    </Button>
                  </Form>
                </Card.Body>
              </Card>
            </Col>

            <Col md={6}>
              <Card>
                <Card.Header className="bg-success text-white">
                  <Card.Title className="mb-0">Áreas Registradas</Card.Title>
                </Card.Header>
                <Card.Body>
                  {areas.length === 0 ? (
                    <p className="text-muted">Nenhuma área registrada</p>
                  ) : (
                    <ListGroup>
                      {areas.map(area => (
                        <ListGroup.Item key={area.id}>
                          <div>
                            <strong>{area.name}</strong> ({area.dimensions})<br />
                            <small className="text-muted">
                              Adesão: MT {area.adhesionValue.toLocaleString()} | 
                              Mensalidade: MT {area.monthlyInstallment.toLocaleString()} |
                              Prazo: {area.paymentPeriodMonths} meses
                            </small>
                          </div>
                        </ListGroup.Item>
                      ))}
                    </ListGroup>
                  )}
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Tab>

        {/* Lots Tab */}
        <Tab eventKey="lots" title="Gestão de Lotes">
          <Row className="mt-4">
            <Col md={6}>
              <Card>
                <Card.Header className="bg-primary text-white">
                  <Card.Title className="mb-0">Criar Novo Lote</Card.Title>
                </Card.Header>
                <Card.Body>
                  <Form onSubmit={handleCreateLot}>
                    <Form.Group className="mb-3">
                      <Form.Label>Área</Form.Label>
                      <Form.Select
                        value={newLot.areaId}
                        onChange={(e) => handleAreaChange(e.target.value)}
                        required
                      >
                        <option value="">-- Selecione uma área --</option>
                        {areas.map(area => (
                          <option key={area.id} value={area.id}>{area.name}</option>
                        ))}
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Nome do Lote</Form.Label>
                      <Form.Control
                        type="text"
                        value={newLot.name}
                        onChange={(e) => setNewLot({ ...newLot, name: e.target.value })}
                        placeholder="Ex: Lote 01"
                        required
                      />
                    </Form.Group>

                    <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                      {loading ? 'Criando...' : 'Criar Lote'}
                    </Button>
                  </Form>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Tab>

        {/* Plots Tab */}
        <Tab eventKey="plots" title="Gestão de Terrenos">
          <Row className="mt-4">
            <Col md={6}>
              <Card>
                <Card.Header className="bg-primary text-white">
                  <Card.Title className="mb-0">Criar Terreno Individual</Card.Title>
                </Card.Header>
                <Card.Body>
                  <Form onSubmit={handleCreatePlot}>
                    <Form.Group className="mb-3">
                      <Form.Label>Área</Form.Label>
                      <Form.Select
                        value={newPlot.areaId}
                        onChange={(e) => {
                          setNewPlot({ ...newPlot, areaId: e.target.value, lotId: '' });
                          handleAreaChange(e.target.value);
                        }}
                        required
                      >
                        <option value="">-- Selecione uma área --</option>
                        {areas.map(area => (
                          <option key={area.id} value={area.id}>{area.name}</option>
                        ))}
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Lote</Form.Label>
                      <Form.Select
                        value={newPlot.lotId}
                        onChange={(e) => setNewPlot({ ...newPlot, lotId: e.target.value })}
                        required
                      >
                        <option value="">-- Selecione um lote --</option>
                        {lots.map(lot => (
                          <option key={lot.id} value={lot.id}>{lot.name}</option>
                        ))}
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Número do Terreno</Form.Label>
                      <Form.Control
                        type="text"
                        value={newPlot.plotNumber}
                        onChange={(e) => setNewPlot({ ...newPlot, plotNumber: e.target.value })}
                        placeholder="Ex: 100"
                        required
                      />
                    </Form.Group>

                    <Button variant="primary" type="submit" className="w-100" disabled={loading}>
                      {loading ? 'Criando...' : 'Criar Terreno'}
                    </Button>
                  </Form>
                </Card.Body>
              </Card>
            </Col>

            <Col md={6}>
              <Card>
                <Card.Header className="bg-success text-white">
                  <Card.Title className="mb-0">Criar Terrenos em Intervalo</Card.Title>
                </Card.Header>
                <Card.Body>
                  <Alert variant="info">
                    Crie múltiplos terrenos de uma só vez especificando um intervalo numérico
                  </Alert>
                  <Form onSubmit={handleCreatePlotRange}>
                    <Form.Group className="mb-3">
                      <Form.Label>Área</Form.Label>
                      <Form.Select
                        value={plotRange.areaId}
                        onChange={(e) => {
                          setPlotRange({ ...plotRange, areaId: e.target.value, lotId: '' });
                          handleAreaChange(e.target.value);
                        }}
                        required
                      >
                        <option value="">-- Selecione uma área --</option>
                        {areas.map(area => (
                          <option key={area.id} value={area.id}>{area.name}</option>
                        ))}
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Lote</Form.Label>
                      <Form.Select
                        value={plotRange.lotId}
                        onChange={(e) => setPlotRange({ ...plotRange, lotId: e.target.value })}
                        required
                      >
                        <option value="">-- Selecione um lote --</option>
                        {lots.map(lot => (
                          <option key={lot.id} value={lot.id}>{lot.name}</option>
                        ))}
                      </Form.Select>
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Número Inicial</Form.Label>
                      <Form.Control
                        type="number"
                        value={plotRange.startNumber}
                        onChange={(e) => setPlotRange({ ...plotRange, startNumber: e.target.value })}
                        placeholder="Ex: 100"
                        required
                      />
                    </Form.Group>

                    <Form.Group className="mb-3">
                      <Form.Label>Número Final</Form.Label>
                      <Form.Control
                        type="number"
                        value={plotRange.endNumber}
                        onChange={(e) => setPlotRange({ ...plotRange, endNumber: e.target.value })}
                        placeholder="Ex: 150"
                        required
                      />
                    </Form.Group>

                    <Button variant="success" type="submit" className="w-100" disabled={loading}>
                      {loading ? 'Criando...' : 'Criar Intervalo'}
                    </Button>
                  </Form>
                </Card.Body>
              </Card>
            </Col>
          </Row>
        </Tab>
      </Tabs>

      <ToastContainer position="top-right" />
    </Container>
  );
}
