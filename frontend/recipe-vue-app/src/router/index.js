import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '@/layouts/MainLayout.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/views/LoginView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'),
      meta: { requiresAuth: false }
    },
    {
      path: '/',
      component: MainLayout,
      meta: { requiresAuth: true },
      children: [
        {
          path: '',
          name: 'home',
          redirect: () => {
            const isAuthenticated = checkAuthentication()
            return isAuthenticated ? '/recipes' : '/login'
          }
        },
        {
          path: 'recipes',
          name: 'recipes',
          component: () => import('@/views/RecipesView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'recipes/:id',
          name: 'recipe-details',
          component: () => import('@/views/RecipeDetailsView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'my-recipes',
          name: 'my-recipes',
          component: () => import('@/views/MyRecipesView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'create-recipe',
          name: 'create-recipe',
          component: () => import('@/views/CreateRecipeView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'profile',
          name: 'profile',
          component: () => import('@/views/ProfileView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'categories',
          name: 'categories',
          component: () => import('@/views/CategoriesView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'tags',
          name: 'tags',
          component: () => import('@/views/TagsView.vue'),
          meta: { requiresAuth: true }
        },
        {
          path: 'ingredients',
          name: 'ingredients',
          component: () => import('@/views/IngredientsView.vue'),
          meta: { requiresAuth: true }
        }
      ]
    },
    // Catch-all route - must be last
    {
      path: '/:pathMatch(.*)*',
      redirect: () => {
        const isAuthenticated = checkAuthentication()
        return isAuthenticated ? '/recipes' : '/login'
      }
    }
  ]
})

// Utility function to check authentication and clean invalid data
const checkAuthentication = () => {
  try {
    const token = localStorage.getItem('token')
    const user = localStorage.getItem('user')
    
    // Check if token is valid
    const hasValidToken = !!(
      token && 
      token !== 'null' && 
      token !== 'undefined' && 
      token.trim() !== ''
    )
    
    // Check if user data is valid
    let hasValidUser = false
    if (user && user !== 'null' && user !== 'undefined') {
      try {
        const userData = JSON.parse(user)
        hasValidUser = !!(userData && (userData.id || userData.Id || userData.username || userData.Username))
      } catch (error) {
        console.error('Invalid user data in localStorage:', error)
        localStorage.removeItem('user')
      }
    }
    
    const isAuthenticated = hasValidToken && hasValidUser
    
    // Clean up invalid data
    if (!isAuthenticated) {
      if (!hasValidToken) localStorage.removeItem('token')
      if (!hasValidUser) localStorage.removeItem('user')
    }
    
    return isAuthenticated
  } catch (error) {
    console.error('Error in checkAuthentication:', error)
    return false
  }
}

// Enhanced navigation guard with better token validation
router.beforeEach((to, from, next) => {
  const isAuthenticated = checkAuthentication()
  
  // If route requires auth and user is not authenticated
  if (to.meta.requiresAuth !== false && !isAuthenticated) {
    next('/login')
    return
  }
  
  // If user is authenticated and trying to access login/register
  if ((to.name === 'login' || to.name === 'register') && isAuthenticated) {
    next('/recipes')
    return
  }
  
  // Allow navigation
  next()
})

export default router 