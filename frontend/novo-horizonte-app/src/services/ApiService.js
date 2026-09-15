import axios from 'axios';
import jwt_decode from 'jwt-decode';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:5001/api';

export class ApiService {
  static getAuthToken() {
    return localStorage.getItem('authToken');
  }

  static setAuthToken(token) {
    localStorage.setItem('authToken', token);
  }

  static removeAuthToken() {
    localStorage.removeItem('authToken');
  }

  static getAuthHeaders() {
    const token = this.getAuthToken();
    return {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    };
  }

  // Auth endpoints
  static async login(email, password) {
    const response = await axios.post(`${API_BASE_URL}/auth/login`, {
      email,
      password
    });
    return response.data;
  }

  static async register(email, password, fullName, phoneNumber, role) {
    const response = await axios.post(`${API_BASE_URL}/auth/register`, {
      email,
      password,
      fullName,
      phoneNumber,
      role
    });
    return response.data;
  }

  // Area endpoints
  static async getAreas() {
    const response = await axios.get(`${API_BASE_URL}/area`);
    return response.data;
  }

  static async createArea(data) {
    const response = await axios.post(`${API_BASE_URL}/area`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  // Lot endpoints
  static async getLotsByArea(areaId) {
    const response = await axios.get(`${API_BASE_URL}/lot/area/${areaId}`);
    return response.data;
  }

  static async createLot(data) {
    const response = await axios.post(`${API_BASE_URL}/lot`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  // Plot endpoints
  static async getAvailablePlotsByLot(lotId) {
    const response = await axios.get(`${API_BASE_URL}/plot/available/lot/${lotId}`);
    return response.data;
  }

  static async getPlotsByAreaAndLot(areaId, lotId) {
    const response = await axios.get(`${API_BASE_URL}/plot/area/${areaId}/lot/${lotId}`);
    return response.data;
  }

  static async createPlot(data) {
    const response = await axios.post(`${API_BASE_URL}/plot`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async createPlotsInRange(data) {
    const response = await axios.post(`${API_BASE_URL}/plot/range`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  // Contract endpoints
  static async reserveTerrains(data) {
    const response = await axios.post(`${API_BASE_URL}/contract/reserve`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async getPendingContracts() {
    const response = await axios.get(`${API_BASE_URL}/contract/pending`, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async getContractDetails(contractId) {
    const response = await axios.get(`${API_BASE_URL}/contract/${contractId}`, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async approveContract(contractId) {
    const response = await axios.post(`${API_BASE_URL}/contract/${contractId}/approve`, {}, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async rejectContract(contractId) {
    const response = await axios.post(`${API_BASE_URL}/contract/${contractId}/reject`, {}, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async getContractPdf(contractId) {
    return `${API_BASE_URL}/contract/${contractId}/pdf`;
  }

  // Payment endpoints
  static async getMyPayments() {
    const response = await axios.get(`${API_BASE_URL}/payment/my-payments`, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async submitPaymentProof(data) {
    const response = await axios.post(`${API_BASE_URL}/payment/submit-proof`, data, {
      headers: this.getAuthHeaders()
    });
    return response.data;
  }

  static async validatePayment(paymentId, paidAmount) {
    const response = await axios.post(
      `${API_BASE_URL}/payment/${paymentId}/validate`,
      paidAmount,
      {
        headers: this.getAuthHeaders()
      }
    );
    return response.data;
  }

  static isTokenExpired(token) {
    try {
      const decoded = jwt_decode(token);
      return decoded.exp * 1000 < Date.now();
    } catch {
      return true;
    }
  }

  static getCurrentUser() {
    const token = this.getAuthToken();
    if (!token || this.isTokenExpired(token)) {
      return null;
    }
    try {
      return jwt_decode(token);
    } catch {
      return null;
    }
  }
}
