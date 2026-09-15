import React, { useEffect, useState } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Navbar, Nav, Container } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import Login from './components/Login';
import AdminDashboard from './components/AdminDashboard';
import OperatorDashboard from './components/OperatorDashboard';
import ClientDashboard from './components/ClientDashboard';
import { ApiService } from './services/ApiService';

function ProtectedRoute({ children, requiredRole }) {
  const user = ApiService.getCurrentUser();
  
  if (!user) {
    return <Navigate to="/login" />;
  }

  if (requiredRole && user.role !== requiredRole) {
    return <Navigate to="/login" />;
  }

  return children;
}

function NavBar() {
  const user = ApiService.getCurrentUser();

  const handleLogout = () => {
    ApiService.removeAuthToken();
    window.location.href = '/login';
  };

  if (!user) return null;

  return (
    <Navbar bg="dark" expand="lg" variant="dark" className="mb-4">
      <Container>
        <Navbar.Brand href="/">Novo Horizonte Digital</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="ms-auto">
            <Nav.Link disabled>{user.name} ({user.role})</Nav.Link>
            <Nav.Link onClick={handleLogout}>Sair</Nav.Link>
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

function App() {
  return (
    <BrowserRouter>
      <NavBar />
      <Routes>
        <Route path="/login" element={<Login />} />
        
        <Route
          path="/admin/dashboard"
          element={
            <ProtectedRoute requiredRole="Admin">
              <AdminDashboard />
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/operator/dashboard"
          element={
            <ProtectedRoute requiredRole="Operator">
              <OperatorDashboard />
            </ProtectedRoute>
          }
        />
        
        <Route
          path="/client/dashboard"
          element={
            <ProtectedRoute requiredRole="Client">
              <ClientDashboard />
            </ProtectedRoute>
          }
        />
        
        <Route path="/" element={<Navigate to="/login" />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
