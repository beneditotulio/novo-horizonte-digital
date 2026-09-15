import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Form, Button, Alert, Card } from 'react-bootstrap';
import { ToastContainer, toast } from 'react-toastify';
import { ApiService } from '../services/ApiService';
import 'react-toastify/dist/ReactToastify.css';

export default function Login() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const response = await ApiService.login(email, password);
      ApiService.setAuthToken(response.token);
      toast.success(`Bem-vindo, ${response.user.fullName}!`);
      
      // Redirect based on role
      if (response.user.role === 'Admin') {
        navigate('/admin/dashboard');
      } else if (response.user.role === 'Operator') {
        navigate('/operator/dashboard');
      } else {
        navigate('/client/dashboard');
      }
    } catch (err) {
      const errorMessage = err.response?.data?.message || 'Falha ao fazer login. Verifique suas credenciais.';
      setError(errorMessage);
      toast.error(errorMessage);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="d-flex justify-content-center align-items-center" style={{ minHeight: '100vh' }}>
      <Card style={{ width: '100%', maxWidth: '400px' }}>
        <Card.Body>
          <h2 className="text-center mb-4">Novo Horizonte Digital</h2>
          <h5 className="text-center mb-4" style={{ color: '#666' }}>Acesso ao Sistema</h5>
          
          {error && <Alert variant="danger">{error}</Alert>}
          
          <Form onSubmit={handleLogin}>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="seu.email@exemplo.com"
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Senha</Form.Label>
              <Form.Control
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Digite sua senha"
                required
              />
            </Form.Group>

            <Button
              variant="primary"
              type="submit"
              className="w-100"
              disabled={isLoading}
            >
              {isLoading ? 'Autenticando...' : 'Entrar'}
            </Button>
          </Form>

          <hr />

          <div style={{ fontSize: '0.9rem', color: '#666' }}>
            <p className="mb-2"><strong>Demo Credentials:</strong></p>
            <p className="mb-2">
              <strong>Admin:</strong><br />
              Email: admin@novo-horizonte.com<br />
              Password: Admin@123
            </p>
            <p className="mb-2">
              <strong>Operator:</strong><br />
              Email: operador@novo-horizonte.com<br />
              Password: Operator@123
            </p>
          </div>
        </Card.Body>
      </Card>
      <ToastContainer position="top-right" />
    </Container>
  );
}
