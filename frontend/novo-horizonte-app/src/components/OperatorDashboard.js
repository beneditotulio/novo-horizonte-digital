import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Card, Table, Button, Badge, Modal, Tab, Tabs } from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import { ApiService } from '../services/ApiService';

export default function OperatorDashboard() {
  const [contracts, setContracts] = useState([]);
  const [selectedContract, setSelectedContract] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadPendingContracts();
  }, []);

  const loadPendingContracts = async () => {
    setLoading(true);
    try {
      const data = await ApiService.getPendingContracts();
      setContracts(data);
    } catch (error) {
      toast.error('Erro ao carregar reservas pendentes');
    } finally {
      setLoading(false);
    }
  };

  const handleViewDetails = async (contract) => {
    try {
      const data = await ApiService.getContractDetails(contract.id);
      setSelectedContract(data);
      setShowModal(true);
    } catch (error) {
      toast.error('Erro ao carregar dados do contrato');
    }
  };

  const handleApproveContract = async () => {
    try {
      await ApiService.approveContract(selectedContract.id);
      toast.success('Contrato aprovado com sucesso!');
      setShowModal(false);
      loadPendingContracts();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao aprovar contrato');
    }
  };

  const handleRejectContract = async () => {
    try {
      await ApiService.rejectContract(selectedContract.id);
      toast.success('Contrato rejeitado com sucesso!');
      setShowModal(false);
      loadPendingContracts();
    } catch (error) {
      toast.error(error.response?.data?.message || 'Erro ao rejeitar contrato');
    }
  };

  const handleDownloadPdf = async (contractId) => {
    try {
      const pdfUrl = await ApiService.getContractPdf(contractId);
      window.open(pdfUrl, '_blank');
    } catch (error) {
      toast.error('Erro ao descarregar PDF');
    }
  };

  return (
    <Container className="py-4">
      <h2 className="mb-4">Dashboard do Operador Comercial</h2>

      <Row>
        <Col>
          <Card>
            <Card.Header className="bg-warning text-dark">
              <Card.Title className="mb-0">Reservas Pendentes de Análise</Card.Title>
            </Card.Header>
            <Card.Body>
              {loading ? (
                <p>Carregando...</p>
              ) : contracts.length === 0 ? (
                <p className="text-muted">Nenhuma reserva pendente</p>
              ) : (
                <Table striped hover responsive>
                  <thead>
                    <tr>
                      <th>Código</th>
                      <th>Cliente</th>
                      <th>Terrenos</th>
                      <th>Valor Adesão</th>
                      <th>Data</th>
                      <th>Ação</th>
                    </tr>
                  </thead>
                  <tbody>
                    {contracts.map(contract => (
                      <tr key={contract.id}>
                        <td><strong>{contract.contractCode}</strong></td>
                        <td>{contract.clientName}</td>
                        <td>{contract.totalPlots}</td>
                        <td>MT {contract.totalAdhesionValue.toLocaleString()}</td>
                        <td>{new Date(contract.createdAt).toLocaleDateString('pt-BR')}</td>
                        <td>
                          <Button
                            size="sm"
                            variant="primary"
                            onClick={() => handleViewDetails(contract)}
                          >
                            Ver Detalhes
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </Table>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {/* Contract Details Modal */}
      <Modal show={showModal} onHide={() => setShowModal(false)} size="lg" scrollable>
        <Modal.Header closeButton>
          <Modal.Title>Detalhes do Contrato</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {selectedContract && (
            <Tabs defaultActiveKey="client" className="mb-3">
              <Tab eventKey="client" title="Dados do Cliente">
                <div className="mt-3">
                  <p><strong>Nome:</strong> {selectedContract.client.fullName}</p>
                  <p><strong>Email:</strong> {selectedContract.client.email}</p>
                  <p><strong>Telefone:</strong> {selectedContract.client.phoneNumber}</p>
                </div>
              </Tab>
              <Tab eventKey="property" title="Imóvel">
                <div className="mt-3">
                  <p><strong>Código do Contrato:</strong> {selectedContract.contractCode}</p>
                  <p><strong>Total de Terrenos:</strong> {selectedContract.totalPlots}</p>
                  <p><strong>Terrenos Selecionados:</strong></p>
                  <div style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fill, minmax(60px, 1fr))',
                    gap: '8px'
                  }}>
                    {selectedContract.plots.map(plot => (
                      <Badge key={plot.id} bg="info" style={{ fontSize: '1rem', padding: '8px' }}>
                        {plot.plotNumber}
                      </Badge>
                    ))}
                  </div>
                </div>
              </Tab>
              <Tab eventKey="financial" title="Financeiro">
                <div className="mt-3">
                  <p><strong>Valor de Adesão:</strong> MT {selectedContract.totalAdhesionValue.toLocaleString()}</p>
                  <p><strong>Valor Total Mensalidades:</strong> MT {selectedContract.totalInstallmentValue.toLocaleString()}</p>
                  <p><strong>Prazo:</strong> {selectedContract.paymentPeriodMonths} meses</p>
                  <p><strong>Status:</strong> <Badge bg="warning">{selectedContract.status}</Badge></p>
                  <hr />
                  <h6>Plano de Mensalidades:</h6>
                  <Table striped size="sm">
                    <thead>
                      <tr>
                        <th>Parcela</th>
                        <th>Valor</th>
                        <th>Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {selectedContract.monthlyPayments.map(payment => (
                        <tr key={payment.id}>
                          <td>{payment.installmentNumber}/{payment.totalInstallments}</td>
                          <td>MT {payment.expectedValue.toLocaleString()}</td>
                          <td>
                            <Badge bg={payment.status === 'Paid' ? 'success' : 'warning'}>
                              {payment.status}
                            </Badge>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </Table>
                </div>
              </Tab>
              <Tab eventKey="contact" title="Contacto Alternativo">
                <div className="mt-3">
                  {selectedContract.alternativeContact ? (
                    <>
                      <p><strong>Nome:</strong> {selectedContract.alternativeContact.fullName}</p>
                      <p><strong>Telefone:</strong> {selectedContract.alternativeContact.phoneNumber}</p>
                      <p><strong>Parentesco:</strong> {selectedContract.alternativeContact.relationship}</p>
                    </>
                  ) : (
                    <p className="text-muted">Nenhum contacto alternativo registrado</p>
                  )}
                </div>
              </Tab>
            </Tabs>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button
            variant="secondary"
            onClick={() => setShowModal(false)}
          >
            Fechar
          </Button>
          <Button
            variant="info"
            onClick={() => handleDownloadPdf(selectedContract.id)}
          >
            Descarregar PDF
          </Button>
          <Button
            variant="danger"
            onClick={handleRejectContract}
          >
            Rejeitar
          </Button>
          <Button
            variant="success"
            onClick={handleApproveContract}
          >
            Aprovar &Contrato
          </Button>
        </Modal.Footer>
      </Modal>

      <ToastContainer position="top-right" />
    </Container>
  );
}
