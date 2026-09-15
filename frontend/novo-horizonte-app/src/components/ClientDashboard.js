import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, ListGroup, Button, Badge, Modal, Form, Alert } from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import { ApiService } from '../services/ApiService';

export default function ClientDashboard() {
  const [areas, setAreas] = useState([]);
  const [lots, setLots] = useState([]);
  const [plots, setPlots] = useState([]);
  const [selectedPlots, setSelectedPlots] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showReservationModal, setShowReservationModal] = useState(false);
  const [showPaymentModal, setShowPaymentModal] = useState(false);
  const [payments, setPayments] = useState([]);
  const [selectedArea, setSelectedArea] = useState(null);
  const [selectedLot, setSelectedLot] = useState(null);
  const [reservationFormData, setReservationFormData] = useState({
    alternativeContactName: '',
    alternativeContactPhone: '',
    relationship: '',
    proofOfPaymentUrl: ''
  });

  useEffect(() => {
    loadAreas();
    loadPayments();
  }, []);

  const loadAreas = async () => {
    try {
      const data = await ApiService.getAreas();
      setAreas(data);
    } catch (error) {
      toast.error('Erro ao carregar áreas');
    }
  };

  const loadPayments = async () => {
    try {
      const data = await ApiService.getMyPayments();
      setPayments(data);
    } catch (error) {
      toast.error('Erro ao carregar pagamentos');
    }
  };

  const handleAreaChange = async (area) => {
    setSelectedArea(area);
    setSelectedLot(null);
    setPlots([]);
    setSelectedPlots([]);
    try {
      const data = await ApiService.getLotsByArea(area.id);
      setLots(data);
    } catch (error) {
      toast.error('Erro ao carregar lotes');
    }
  };

  const handleLotChange = async (lot) => {
    setSelectedLot(lot);
    setSelectedPlots([]);
    setLoading(true);
    try {
      const data = await ApiService.getPlotsByAreaAndLot(selectedArea.id, lot.id);
      setPlots(data);
    } catch (error) {
      toast.error('Erro ao carregar terrenos');
    } finally {
      setLoading(false);
    }
  };

  const togglePlotSelection = (plotId) => {
    setSelectedPlots(prev =>
      prev.includes(plotId) ? prev.filter(id => id !== plotId) : [...prev, plotId]
    );
  };

  const handleReservationSubmit = async (e) => {
    e.preventDefault();
    if (selectedPlots.length === 0) {
      toast.error('Selecione pelo menos um terreno');
      return;
    }

    try {
      const response = await ApiService.reserveTerrains({
        plotIds: selectedPlots,
        proofOfPaymentUrl: reservationFormData.proofOfPaymentUrl,
        alternativeContactName: reservationFormData.alternativeContactName,
        alternativeContactPhone: reservationFormData.alternativeContactPhone,
        relationship: reservationFormData.relationship
      });
      toast.success('Reserva criada com sucesso!');
      setShowReservationModal(false);
      setSelectedPlots([]);
      setReservationFormData({
        alternativeContactName: '',
        alternativeContactPhone: '',
        relationship: '',
        proofOfPaymentUrl: ''
      });
      loadPayments();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao criar reserva');
    }
  };

  return (
    <Container className="py-4">
      <h2 className="mb-4">Dashboard do Cliente</h2>

      <Row>
        <Col md={6}>
          <Card className="mb-4">
            <Card.Header className="bg-primary text-white">
              <Card.Title className="mb-0">Selecione Terrenos</Card.Title>
            </Card.Header>
            <Card.Body>
              <Form.Group className="mb-3">
                <Form.Label>Categoria de Terreno</Form.Label>
                <Form.Select onChange={(e) => {
                  const area = areas.find(a => a.id === parseInt(e.target.value));
                  if (area) handleAreaChange(area);
                }}>
                  <option value="">-- Selecione uma categoria --</option>
                  {areas.map(area => (
                    <option key={area.id} value={area.id}>
                      {area.name} ({area.dimensions}) - MT {area.adhesionValue.toLocaleString()}
                    </option>
                  ))}
                </Form.Select>
              </Form.Group>

              {selectedArea && (
                <Form.Group className="mb-3">
                  <Form.Label>Lote</Form.Label>
                  <Form.Select onChange={(e) => {
                    const lot = lots.find(l => l.id === parseInt(e.target.value));
                    if (lot) handleLotChange(lot);
                  }}>
                    <option value="">-- Selecione um lote --</option>
                    {lots.map(lot => (
                      <option key={lot.id} value={lot.id}>{lot.name}</option>
                    ))}
                  </Form.Select>
                </Form.Group>
              )}

              {selectedLot && (
                <div>
                  <Form.Label>Terrenos Disponíveis</Form.Label>
                  <div className="plot-grid" style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fill, minmax(80px, 1fr))',
                    gap: '8px',
                    marginBottom: '15px'
                  }}>
                    {plots.map(plot => (
                      <div
                        key={plot.id}
                        onClick={() => togglePlotSelection(plot.id)}
                        style={{
                          padding: '10px',
                          border: selectedPlots.includes(plot.id) ? '2px solid #007bff' : '1px solid #ddd',
                          backgroundColor: selectedPlots.includes(plot.id) ? '#e7f3ff' : '#fff',
                          borderRadius: '4px',
                          cursor: 'pointer',
                          textAlign: 'center',
                          fontWeight: 'bold'
                        }}
                      >
                        {plot.plotNumber}
                      </div>
                    ))}
                  </div>

                  {selectedPlots.length > 0 && (
                    <Alert variant="info">
                      {selectedPlots.length} terreno(s) selecionado(s)
                    </Alert>
                  )}

                  <Button
                    variant="success"
                    className="w-100"
                    onClick={() => setShowReservationModal(true)}
                    disabled={selectedPlots.length === 0}
                  >
                    Reservar Terreno(s)
                  </Button>
                </div>
              )}
            </Card.Body>
          </Card>
        </Col>

        <Col md={6}>
          <Card>
            <Card.Header className="bg-info text-white">
              <Card.Title className="mb-0">Minhas Mensalidades</Card.Title>
            </Card.Header>
            <Card.Body>
              {payments.length === 0 ? (
                <p className="text-muted">Nenhuma mensalidade registrada</p>
              ) : (
                <ListGroup>
                  {payments.map(payment => (
                    <ListGroup.Item key={payment.id}>
                      <Row className="align-items-center">
                        <Col>
                          <div>
                            <strong>Parcela {payment.installmentNumber}/{payment.totalInstallments}</strong><br />
                            <small>{new Date(payment.monthYear).toLocaleDateString('pt-BR', { year: 'numeric', month: 'long' })}</small>
                          </div>
                        </Col>
                        <Col md={4} className="text-end">
                          <Badge bg={payment.status === 'Paid' ? 'success' : 'warning'}>
                            {payment.status === 'Paid' ? 'Pago' : 'Pendente'}
                          </Badge>
                          <div>MT {payment.expectedValue.toLocaleString()}</div>
                        </Col>
                      </Row>
                    </ListGroup.Item>
                  ))}
                </ListGroup>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Reservation Modal */}
      <Modal show={showReservationModal} onHide={() => setShowReservationModal(false)} size="lg">
        <Modal.Header closeButton>
          <Modal.Title>Preencher Formulário de Reserva</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleReservationSubmit}>
            <Form.Group className="mb-3">
              <Form.Label>Nome do Contacto Alternativo</Form.Label>
              <Form.Control
                type="text"
                value={reservationFormData.alternativeContactName}
                onChange={(e) => setReservationFormData({
                  ...reservationFormData,
                  alternativeContactName: e.target.value
                })}
                placeholder="Nome do familiar/contacto"
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Telefone do Contacto</Form.Label>
              <Form.Control
                type="tel"
                value={reservationFormData.alternativeContactPhone}
                onChange={(e) => setReservationFormData({
                  ...reservationFormData,
                  alternativeContactPhone: e.target.value
                })}
                placeholder="+258 ..."
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Grau de Parentesco</Form.Label>
              <Form.Select
                value={reservationFormData.relationship}
                onChange={(e) => setReservationFormData({
                  ...reservationFormData,
                  relationship: e.target.value
                })}
                required
              >
                <option value="">-- Selecione --</option>
                <option value="Pai/Mãe">Pai/Mãe</option>
                <option value="Cônjuge">Cônjuge</option>
                <option value="Irmão/Irmã">Irmão/Irmã</option>
                <option value="Filho/Filha">Filho/Filha</option>
                <option value="Outro">Outro</option>
              </Form.Select>
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Comprovativo de Pagamento (URL)</Form.Label>
              <Form.Control
                type="text"
                value={reservationFormData.proofOfPaymentUrl}
                onChange={(e) => setReservationFormData({
                  ...reservationFormData,
                  proofOfPaymentUrl: e.target.value
                })}
                placeholder="https://..."
                required
              />
            </Form.Group>

            <Button variant="success" type="submit" className="w-100">
              Enviar Reserva
            </Button>
          </Form>
        </Modal.Body>
      </Modal>

      <ToastContainer position="top-right" />
    </Container>
  );
}
