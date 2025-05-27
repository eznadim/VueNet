<template>
  <div class="my-recipes-container">
    <h1>My Recipes</h1>
    <div v-if="loading" class="loading">Loading your recipes...</div>
    <div v-else>
      <div v-if="recipes.length" class="recipes-grid">
        <div v-for="recipe in recipes" :key="recipe.id" class="recipe-card">
          <img :src="recipe.imageUrl" :alt="recipe.title" class="recipe-image" />
          <div class="recipe-content">
            <h3 class="recipe-title">{{ recipe.title }}</h3>
            <p class="recipe-description">{{ recipe.description }}</p>
            <div class="recipe-meta">
              <span>{{ recipe.cookTimeMinutes || 0 }} mins</span>
              <span>{{ recipe.difficulty }}</span>
            </div>
            <router-link :to="'/recipes/' + recipe.id" class="view-recipe">View</router-link>
            <button class="edit-btn" @click="editRecipe(recipe.id)">Edit</button>
            <button class="delete-btn" @click="deleteRecipe(recipe.id)">Delete</button>
          </div>
        </div>
      </div>
      <div v-else class="no-recipes">You haven't created any recipes yet.</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'

const router = useRouter()
const recipes = ref([])
const loading = ref(true)

const fetchMyRecipes = async () => {
  try {
    loading.value = true
    const response = await api.get('/recipes/my')
    recipes.value = response.data
  } catch (error) {
    recipes.value = []
  } finally {
    loading.value = false
  }
}

const editRecipe = (id) => {
  router.push(`/recipes/${id}/edit`)
}

const deleteRecipe = async (id) => {
  if (confirm('Are you sure you want to delete this recipe?')) {
    try {
      await api.delete(`/recipes/${id}`)
      recipes.value = recipes.value.filter(r => r.id !== id)
    } catch (error) {
      alert('Failed to delete recipe.')
    }
  }
}

onMounted(() => {
  fetchMyRecipes()
})
</script>

<style scoped>
.my-recipes-container {
  max-width: 1200px;
  margin: 2rem auto;
  padding: 1rem;
}

h1 {
  color: #333;
  margin-bottom: 2rem;
}

.recipes-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.recipe-card {
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s;
  display: flex;
  flex-direction: column;
}

.recipe-card:hover {
  transform: translateY(-4px);
}

.recipe-image {
  width: 100%;
  height: 180px;
  object-fit: cover;
}

.recipe-content {
  padding: 1.2rem;
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.recipe-title {
  margin: 0 0 0.3rem 0;
  color: #333;
}

.recipe-description {
  color: #666;
  margin-bottom: 0.5rem;
  font-size: 0.95rem;
}

.recipe-meta {
  display: flex;
  gap: 1rem;
  color: #888;
  font-size: 0.9rem;
  margin-bottom: 0.5rem;
}

.view-recipe {
  color: #4CAF50;
  text-decoration: none;
  margin-bottom: 0.5rem;
}

.edit-btn, .delete-btn {
  background: none;
  border: none;
  color: #4CAF50;
  cursor: pointer;
  margin-right: 0.5rem;
  font-size: 0.95rem;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  transition: background 0.2s;
}

.edit-btn:hover {
  background: #e8f5e9;
}

.delete-btn {
  color: #dc3545;
}

.delete-btn:hover {
  background: #fdecea;
}

.no-recipes {
  text-align: center;
  color: #888;
  padding: 2rem;
}

.loading {
  text-align: center;
  color: #888;
  padding: 2rem;
}
</style> 