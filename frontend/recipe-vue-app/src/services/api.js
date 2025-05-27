import axios from 'axios'

const API_BASE_URL = 'http://localhost:5027/api' // Adjust this to match your backend URL

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor to handle auth errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth API
export const authAPI = {
  login: (credentials) => api.post('/auth/login', credentials),
  register: (userData) => api.post('/auth/register', userData),
  logout: () => api.post('/auth/logout')
}

// Recipes API
export const recipesAPI = {
  getAll: (params = {}) => api.get('/recipes', { params }),
  getById: (id) => api.get(`/recipes/${id}`),
  create: (recipeData) => api.post('/recipes', recipeData),
  update: (id, recipeData) => api.put(`/recipes/${id}`, recipeData),
  delete: (id) => api.delete(`/recipes/${id}`),
  getMyRecipes: () => api.get('/recipes/my-recipes')
}

// Categories API
export const categoriesAPI = {
  getAll: () => api.get('/categories'),
  getById: (id) => api.get(`/categories/${id}`),
  create: (categoryData) => api.post('/categories', categoryData),
  update: (id, categoryData) => api.put(`/categories/${id}`, categoryData),
  delete: (id) => api.delete(`/categories/${id}`)
}

// Tags API
export const tagsAPI = {
  getAll: () => api.get('/tags'),
  getById: (id) => api.get(`/tags/${id}`),
  create: (tagData) => api.post('/tags', tagData),
  update: (id, tagData) => api.put(`/tags/${id}`, tagData),
  delete: (id) => api.delete(`/tags/${id}`)
}

// Ingredients API
export const ingredientsAPI = {
  getAll: (searchTerm = '') => api.get('/ingredients', { params: { searchTerm } }),
  getById: (id) => api.get(`/ingredients/${id}`),
  create: (ingredientData) => api.post('/ingredients', ingredientData),
  update: (id, ingredientData) => api.put(`/ingredients/${id}`, ingredientData),
  delete: (id) => api.delete(`/ingredients/${id}`)
}

// Ratings API
export const ratingsAPI = {
  getRecipeRatings: (recipeId) => api.get(`/ratings/recipe/${recipeId}`),
  getRecipeAverage: (recipeId) => api.get(`/ratings/recipe/${recipeId}/average`),
  create: (ratingData) => api.post('/ratings', ratingData),
  update: (id, ratingData) => api.put(`/ratings/${id}`, ratingData),
  delete: (id) => api.delete(`/ratings/${id}`)
}

// Users API
export const usersAPI = {
  getProfile: () => api.get('/users/profile'),
  updateProfile: (userData) => api.put('/users/profile', userData),
  getUser: (id) => api.get(`/users/${id}`),
  getUserRecipes: (id) => api.get(`/users/${id}/recipes`)
}

export default api