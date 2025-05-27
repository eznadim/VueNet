<template>
  <div class="register-container">
    <div class="background-overlay"></div>
    <div class="register-card">
      <div class="logo-section">
        <div class="logo-icon">👨‍🍳</div>
        <h1>Join Recipe Haven</h1>
        <p class="subtitle">Start your culinary adventure today</p>
      </div>
      
      <form @submit.prevent="handleRegister" class="register-form">
        <div class="form-group">
          <label for="username">
            <span class="label-icon">👤</span>
            Username
          </label>
          <input
            type="text"
            id="username"
            v-model="username"
            required
            placeholder="Choose a username"
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="email">
            <span class="label-icon">📧</span>
            Email
          </label>
          <input
            type="email"
            id="email"
            v-model="email"
            required
            placeholder="Enter your email"
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="password">
            <span class="label-icon">🔒</span>
            Password
          </label>
          <input
            type="password"
            id="password"
            v-model="password"
            required
            placeholder="Create a password"
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label for="confirmPassword">
            <span class="label-icon">🔐</span>
            Confirm Password
          </label>
          <input
            type="password"
            id="confirmPassword"
            v-model="confirmPassword"
            required
            placeholder="Confirm your password"
            class="form-input"
          />
        </div>
        
        <button type="submit" class="register-button" :disabled="loading">
          <span v-if="loading" class="loading-spinner"></span>
          {{ loading ? 'Creating Account...' : 'Create Account' }}
        </button>
        
        <div class="error-message" v-if="error">
          <span class="error-icon">⚠️</span>
          {{ error }}
        </div>
        
        <div class="divider">
          <span>or</span>
        </div>
        
        <p class="login-link">
          Already have an account?
          <router-link to="/login" class="login-btn">Sign In</router-link>
        </p>
      </form>
    </div>
    
    <!-- Floating recipe elements for decoration -->
    <div class="floating-elements">
      <div class="floating-item item-1">🥗</div>
      <div class="floating-item item-2">🍝</div>
      <div class="floating-item item-3">🥘</div>
      <div class="floating-item item-4">🍰</div>
      <div class="floating-item item-5">🥖</div>
      <div class="floating-item item-6">🍕</div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'

const router = useRouter()
const username = ref('')
const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const loading = ref(false)
const error = ref('')

const handleRegister = async () => {
  if (!username.value || !email.value || !password.value) {
    error.value = 'All fields are required.'
    return
  }
  if (password.value.length < 6) {
    error.value = 'Password must be at least 6 characters.'
    return
  }
  if (password.value !== confirmPassword.value) {
    error.value = 'Passwords do not match.'
    return
  }

  try {
    loading.value = true
    error.value = ''
    const response = await api.post('/auth/register', {
      username: username.value,
      email: email.value,
      password: password.value
    })
    
    router.push('/login')
  } catch (err) {
    if (err.response?.data?.message) {
      error.value = err.response.data.message
    } else if (err.response?.status === 400) {
      error.value = 'Username or email already exists.'
    } else {
      error.value = 'Failed to create account. Please try again.'
    }
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.register-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  background-image: 
    radial-gradient(circle at 20% 80%, rgba(120, 119, 198, 0.3) 0%, transparent 50%),
    radial-gradient(circle at 80% 20%, rgba(255, 177, 153, 0.3) 0%, transparent 50%),
    radial-gradient(circle at 40% 40%, rgba(120, 119, 198, 0.2) 0%, transparent 50%);
  overflow: hidden;
  padding: 1rem;
}

.background-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.1);
  z-index: 1;
}

.register-card {
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(20px);
  padding: 3rem 2.5rem;
  border-radius: 20px;
  box-shadow: 
    0 20px 40px rgba(0, 0, 0, 0.1),
    0 0 0 1px rgba(255, 255, 255, 0.2);
  width: 100%;
  max-width: 450px;
  min-width: 320px;
  display: flex;
  flex-direction: column;
  align-items: center;
  position: relative;
  z-index: 3;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.logo-section {
  text-align: center;
  margin-bottom: 2.5rem;
}

.logo-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.1));
  animation: bounce 2s infinite;
}

@keyframes bounce {
  0%, 20%, 50%, 80%, 100% {
    transform: translateY(0);
  }
  40% {
    transform: translateY(-10px);
  }
  60% {
    transform: translateY(-5px);
  }
}

h1 {
  color: #2c3e50;
  margin-bottom: 0.5rem;
  font-family: 'Georgia', serif;
  font-size: 2rem;
  font-weight: 600;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.subtitle {
  color: #7f8c8d;
  font-size: 1rem;
  font-style: italic;
  margin-bottom: 0;
}

.register-form {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

label {
  font-weight: 600;
  color: #2c3e50;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.95rem;
}

.label-icon {
  font-size: 1.1rem;
}

.form-input {
  padding: 1rem;
  border: 2px solid #e1e8ed;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: rgba(255, 255, 255, 0.8);
}

.form-input:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.1);
  background: rgba(255, 255, 255, 1);
}

.form-input::placeholder {
  color: #95a5a6;
}

.register-button {
  background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
  color: white;
  padding: 1rem;
  border: none;
  border-radius: 12px;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  margin-top: 0.5rem;
}

.register-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(76, 175, 80, 0.3);
}

.register-button:active {
  transform: translateY(0);
}

.register-button:disabled {
  background: linear-gradient(135deg, #bdc3c7 0%, #95a5a6 100%);
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.loading-spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: #fff;
  animation: spin 1s linear infinite;
  margin-right: 0.5rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.error-message {
  background: rgba(231, 76, 60, 0.1);
  color: #e74c3c;
  padding: 0.75rem;
  border-radius: 8px;
  text-align: center;
  border: 1px solid rgba(231, 76, 60, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.error-icon {
  font-size: 1.1rem;
}

.divider {
  text-align: center;
  margin: 1.5rem 0;
  position: relative;
}

.divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: linear-gradient(to right, transparent, #e1e8ed, transparent);
}

.divider span {
  padding: 0 1rem;
  background: rgba(255, 255, 255, 0.95);
  color: #7f8c8d;
  font-size: 0.9rem;
  position: relative;
  z-index: 1;
}

.login-link {
  text-align: center;
  margin-top: 0.5rem;
  color: #7f8c8d;
}

.login-btn {
  color: #4CAF50;
  text-decoration: none;
  font-weight: 600;
  margin-left: 0.5rem;
  transition: all 0.2s ease;
}

.login-btn:hover {
  color: #45a049;
  text-decoration: underline;
}

.floating-elements {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 2;
}

.floating-item {
  position: absolute;
  font-size: 2rem;
  opacity: 0.6;
  animation: float 6s ease-in-out infinite;
}

@keyframes float {
  0%, 100% {
    transform: translateY(0px) rotate(0deg);
  }
  50% {
    transform: translateY(-20px) rotate(180deg);
  }
}

.item-1 {
  top: 10%;
  left: 8%;
  animation-delay: 0s;
}

.item-2 {
  top: 70%;
  left: 12%;
  animation-delay: 1s;
}

.item-3 {
  top: 20%;
  left: 88%;
  animation-delay: 2s;
}

.item-4 {
  top: 55%;
  left: 85%;
  animation-delay: 3s;
}

.item-5 {
  top: 85%;
  left: 80%;
  animation-delay: 4s;
}

.item-6 {
  top: 40%;
  left: 5%;
  animation-delay: 5s;
}

/* Responsive design for different screen sizes */
@media (max-width: 480px) {
  .register-container {
    padding: 0.5rem;
  }
  
  .register-card {
    padding: 2rem 1.5rem;
    min-width: 280px;
  }
  
  .logo-icon {
    font-size: 3rem;
  }
  
  h1 {
    font-size: 1.5rem;
  }
  
  .floating-item {
    font-size: 1.5rem;
  }
  
  .register-form {
    gap: 1.2rem;
  }
}

@media (min-width: 481px) and (max-width: 768px) {
  .register-card {
    max-width: 480px;
    padding: 3rem 2rem;
  }
  
  .register-form {
    gap: 1.3rem;
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .register-card {
    max-width: 520px;
    padding: 3.5rem 3rem;
  }
  
  .floating-item {
    font-size: 2.5rem;
  }
}

@media (min-width: 1025px) {
  .register-container {
    padding: 2rem;
  }
  
  .register-card {
    max-width: 550px;
    padding: 4rem 3.5rem;
  }
  
  .logo-icon {
    font-size: 5rem;
  }
  
  h1 {
    font-size: 2.5rem;
  }
  
  .subtitle {
    font-size: 1.1rem;
  }
  
  .floating-item {
    font-size: 3rem;
  }
  
  /* Add more floating items for larger screens */
  .floating-elements::after {
    content: '🥨';
    position: absolute;
    top: 25%;
    left: 3%;
    font-size: 3rem;
    opacity: 0.4;
    animation: float 8s ease-in-out infinite;
    animation-delay: 6s;
  }
  
  .floating-elements::before {
    content: '🍪';
    position: absolute;
    top: 75%;
    left: 92%;
    font-size: 3rem;
    opacity: 0.4;
    animation: float 7s ease-in-out infinite;
    animation-delay: 7s;
  }
}

@media (min-width: 1440px) {
  .register-card {
    max-width: 600px;
    padding: 4.5rem 4rem;
  }
  
  .form-input {
    padding: 1.2rem;
    font-size: 1.1rem;
  }
  
  .register-button {
    padding: 1.2rem;
    font-size: 1.2rem;
  }
  
  .register-form {
    gap: 1.8rem;
  }
}

/* Ultra-wide screens */
@media (min-width: 1920px) {
  .register-container {
    padding: 3rem;
  }
  
  .register-card {
    max-width: 650px;
    padding: 5rem 4.5rem;
  }
  
  .logo-icon {
    font-size: 6rem;
  }
  
  h1 {
    font-size: 3rem;
  }
  
  .subtitle {
    font-size: 1.3rem;
  }
  
  .floating-item {
    font-size: 4rem;
  }
  
  .register-form {
    gap: 2rem;
  }
  
  .form-input {
    padding: 1.4rem;
    font-size: 1.2rem;
  }
  
  .register-button {
    padding: 1.4rem;
    font-size: 1.3rem;
  }
}
</style> 