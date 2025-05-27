<template>
  <div class="profile-container">
    <h1>My Profile</h1>
    <form class="profile-form" @submit.prevent="saveProfile">
      <div class="form-group">
        <label for="name">Name</label>
        <input id="name" v-model="name" type="text" required />
      </div>
      <div class="form-group">
        <label for="email">Email</label>
        <input id="email" v-model="email" type="email" required />
      </div>
      <button type="submit" class="save-btn" :disabled="loading">{{ loading ? 'Saving...' : 'Save Changes' }}</button>
      <button type="button" class="change-password-btn" @click="showChangePassword = true">Change Password</button>
      <p v-if="success" class="success-message">Profile updated successfully!</p>
      <p v-if="error" class="error-message">{{ error }}</p>
    </form>

    <div v-if="showChangePassword" class="modal-overlay">
      <div class="modal">
        <h2>Change Password</h2>
        <form @submit.prevent="changePassword">
          <div class="form-group">
            <label for="currentPassword">Current Password</label>
            <input id="currentPassword" v-model="currentPassword" type="password" required />
          </div>
          <div class="form-group">
            <label for="newPassword">New Password</label>
            <input id="newPassword" v-model="newPassword" type="password" required />
          </div>
          <button type="submit" class="save-btn" :disabled="loading">Change Password</button>
          <button type="button" class="cancel-btn" @click="showChangePassword = false">Cancel</button>
          <p v-if="passwordSuccess" class="success-message">Password changed successfully!</p>
          <p v-if="passwordError" class="error-message">{{ passwordError }}</p>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import api from '@/services/api'

const user = JSON.parse(localStorage.getItem('user') || '{}')
const name = ref(user.name || '')
const email = ref(user.email || '')
const loading = ref(false)
const success = ref(false)
const error = ref('')

const showChangePassword = ref(false)
const currentPassword = ref('')
const newPassword = ref('')
const passwordSuccess = ref(false)
const passwordError = ref('')

const saveProfile = async () => {
  try {
    loading.value = true
    error.value = ''
    success.value = false
    const response = await api.put('/users/profile', { name: name.value, email: email.value })
    localStorage.setItem('user', JSON.stringify(response.data))
    success.value = true
  } catch (err) {
    error.value = err.response?.data?.message || 'Failed to update profile'
  } finally {
    loading.value = false
  }
}

const changePassword = async () => {
  try {
    loading.value = true
    passwordError.value = ''
    passwordSuccess.value = false
    await api.put('/users/change-password', { currentPassword: currentPassword.value, newPassword: newPassword.value })
    passwordSuccess.value = true
    currentPassword.value = ''
    newPassword.value = ''
  } catch (err) {
    passwordError.value = err.response?.data?.message || 'Failed to change password'
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.profile-container {
  max-width: 500px;
  margin: 2rem auto;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  padding: 2rem;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
}

.profile-form {
  display: flex;
  flex-direction: column;
  gap: 1.2rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

label {
  font-weight: 500;
  color: #555;
}

input {
  padding: 0.7rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 1rem;
}

.save-btn {
  background-color: #4CAF50;
  color: white;
  padding: 0.7rem;
  border: none;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  margin-bottom: 0.5rem;
}

.save-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.change-password-btn {
  background: none;
  border: 1px solid #4CAF50;
  color: #4CAF50;
  padding: 0.7rem;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  margin-bottom: 0.5rem;
}

.success-message {
  color: #28a745;
  margin-top: 0.5rem;
}

.error-message {
  color: #dc3545;
  margin-top: 0.5rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0,0,0,0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 8px;
  padding: 2rem;
  min-width: 350px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.15);
}

.cancel-btn {
  background: none;
  border: 1px solid #dc3545;
  color: #dc3545;
  padding: 0.7rem;
  border-radius: 4px;
  font-size: 1rem;
  cursor: pointer;
  margin-left: 0.5rem;
}
</style> 