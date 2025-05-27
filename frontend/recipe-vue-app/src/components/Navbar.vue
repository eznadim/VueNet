<template>
  <nav class="navbar">
    <div class="navbar-brand">
      <router-link to="/recipes" class="logo">Recipe Platform</router-link>
    </div>
    
    <div class="navbar-menu">
      <router-link to="/recipes" class="nav-link">Recipes</router-link>
      <router-link to="/my-recipes" class="nav-link">My Recipes</router-link>
      <router-link to="/create-recipe" class="nav-link">Create Recipe</router-link>
      
      <!-- Management dropdown -->
      <div class="dropdown" @click="toggleManagementMenu" ref="managementMenuRef">
        <span class="nav-link dropdown-trigger">
          Manage
          <svg class="dropdown-icon" :class="{ 'rotated': isManagementMenuOpen }" width="12" height="12" viewBox="0 0 12 12" fill="currentColor">
            <path d="M6 8L2 4h8L6 8z"/>
          </svg>
        </span>
        
        <div class="dropdown-menu" v-if="isManagementMenuOpen">
          <router-link to="/categories" class="dropdown-item">Categories</router-link>
          <router-link to="/tags" class="dropdown-item">Tags</router-link>
          <router-link to="/ingredients" class="dropdown-item">Ingredients</router-link>
        </div>
      </div>
    </div>

    <div class="navbar-end">
      <div v-if="user" class="user-menu" @click="toggleUserMenu" ref="userMenuRef">
        <div class="user-info">
          <span class="user-name">{{ user?.username || user?.name || 'User' }}</span>
          <img :src="user?.avatar || '/default-avatar.png'" alt="User avatar" class="avatar" />
        </div>
        
        <div class="dropdown-menu" v-if="isUserMenuOpen">
          <router-link to="/profile" class="dropdown-item">Profile</router-link>
          <button @click="handleLogout" class="dropdown-item">Logout</button>
        </div>
      </div>
      <div v-else class="auth-links">
        <router-link to="/login" class="nav-link">Login</router-link>
        <router-link to="/register" class="nav-link">Register</router-link>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const isUserMenuOpen = ref(false)
const isManagementMenuOpen = ref(false)
const userMenuRef = ref(null)
const managementMenuRef = ref(null)
const user = ref(null)

// Function to get user from localStorage
const getUserFromStorage = () => {
  try {
    const userData = localStorage.getItem('user')
    if (!userData || userData === 'undefined' || userData === 'null') {
      return null
    }
    return JSON.parse(userData)
  } catch (error) {
    console.error('Error parsing user data from localStorage:', error)
    localStorage.removeItem('user')
    return null
  }
}

// Initialize user data
const initializeUser = () => {
  user.value = getUserFromStorage()
}

// Listen for storage changes
const handleStorageChange = (event) => {
  if (event.key === 'user') {
    user.value = getUserFromStorage()
  }
}

const toggleUserMenu = () => {
  isUserMenuOpen.value = !isUserMenuOpen.value
  isManagementMenuOpen.value = false
}

const toggleManagementMenu = () => {
  isManagementMenuOpen.value = !isManagementMenuOpen.value
  isUserMenuOpen.value = false
}

const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  user.value = null
  isUserMenuOpen.value = false
  router.push('/login')
}

const handleClickOutside = (event) => {
  if (userMenuRef.value && !userMenuRef.value.contains(event.target)) {
    isUserMenuOpen.value = false
  }
  if (managementMenuRef.value && !managementMenuRef.value.contains(event.target)) {
    isManagementMenuOpen.value = false
  }
}

onMounted(() => {
  initializeUser()
  document.addEventListener('click', handleClickOutside)
  window.addEventListener('storage', handleStorageChange)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
  window.removeEventListener('storage', handleStorageChange)
})
</script>

<style scoped>
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 2rem;
  background-color: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.navbar-brand {
  font-size: 1.5rem;
  font-weight: bold;
}

.logo {
  color: #4CAF50;
  text-decoration: none;
}

.navbar-menu {
  display: flex;
  align-items: center;
  gap: 2rem;
}

.nav-link {
  color: #333;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s;
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.nav-link:hover {
  color: #4CAF50;
}

.dropdown {
  position: relative;
}

.dropdown-trigger {
  cursor: pointer;
  user-select: none;
}

.dropdown-icon {
  transition: transform 0.2s ease;
}

.dropdown-icon.rotated {
  transform: rotate(180deg);
}

.navbar-end {
  position: relative;
}

.auth-links {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-menu {
  cursor: pointer;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-name {
  font-weight: 500;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
}

.dropdown-menu {
  position: absolute;
  top: 100%;
  right: 0;
  background-color: white;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  padding: 0.5rem 0;
  min-width: 150px;
  margin-top: 0.5rem;
  z-index: 1000;
}

.dropdown .dropdown-menu {
  right: auto;
  left: 0;
}

.dropdown-item {
  display: block;
  padding: 0.75rem 1rem;
  color: #333;
  text-decoration: none;
  transition: background-color 0.2s;
  font-size: 0.9rem;
}

.dropdown-item:hover {
  background-color: #f8f9fa;
  color: #4CAF50;
}

button.dropdown-item {
  width: 100%;
  text-align: left;
  border: none;
  background: none;
  font-size: 0.9rem;
  cursor: pointer;
}
</style> 