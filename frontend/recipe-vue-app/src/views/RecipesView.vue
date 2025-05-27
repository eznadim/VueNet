<template>
  <div class="recipes-container">
    <div class="recipes-header">
      <h1>Recipes</h1>
      <div class="search-bar">
        <input
          type="text"
          v-model="searchQuery"
          placeholder="Search recipes..."
          @input="handleSearch"
        />
      </div>
    </div>

    <div class="recipes-grid" v-if="!loading">
      <div v-for="recipe in recipes" :key="recipe.id" class="recipe-card">
        <img :src="recipe.imageUrl || '/placeholder-recipe.jpg'" :alt="recipe.title" class="recipe-image" />
        <div class="recipe-content">
          <h3 class="recipe-title">{{ recipe.title }}</h3>
          <p class="recipe-description">{{ recipe.description }}</p>
          <div class="recipe-meta">
            <span class="cooking-time" v-if="recipe.cookTimeMinutes">
              <i class="fas fa-clock"></i> {{ recipe.cookTimeMinutes }} mins
            </span>
            <span class="prep-time" v-if="recipe.prepTimeMinutes">
              <i class="fas fa-hourglass-start"></i> {{ recipe.prepTimeMinutes }} mins prep
            </span>
            <span class="difficulty">
              <i class="fas fa-signal"></i> {{ recipe.difficulty }}
            </span>
            <span class="servings" v-if="recipe.servings">
              <i class="fas fa-users"></i> {{ recipe.servings }} servings
            </span>
          </div>
          <div class="recipe-tags" v-if="recipe.tags && recipe.tags.length > 0">
            <span v-for="tag in recipe.tags" :key="tag.id" class="tag" :style="{ backgroundColor: tag.color }">
              {{ tag.name }}
            </span>
          </div>
          <router-link :to="'/recipes/' + recipe.id" class="view-recipe">
            View Recipe
          </router-link>
        </div>
      </div>
    </div>

    <div v-else class="loading">
      Loading recipes...
    </div>

    <div v-if="!loading && recipes.length === 0" class="no-recipes">
      No recipes found
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { recipesAPI } from '@/services/api'

const recipes = ref([])
const loading = ref(true)
const searchQuery = ref('')
let searchTimeout = null

const fetchRecipes = async () => {
  try {
    loading.value = true
    const response = await recipesAPI.getAll()
    // Map each recipe to use lowercase keys
    recipes.value = response.data.map(r => ({
      id: r.id || r.Id,
      title: r.title || r.Title,
      description: r.description || r.Description,
      imageUrl: r.imageUrl || r.ImageUrl,
      cookTimeMinutes: r.cookTimeMinutes || r.CookTimeMinutes,
      prepTimeMinutes: r.prepTimeMinutes || r.PrepTimeMinutes,
      difficulty: r.difficulty || r.Difficulty,
      servings: r.servings || r.Servings,
      tags: r.tags || r.Tags || [],
    }))
  } catch (error) {
    console.error('Failed to fetch recipes:', error)
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  if (searchTimeout) {
    clearTimeout(searchTimeout)
  }
  
  searchTimeout = setTimeout(async () => {
    try {
      loading.value = true
      const response = await recipesAPI.getAll({ searchTerm: searchQuery.value })
      recipes.value = response.data
    } catch (error) {
      console.error('Failed to search recipes:', error)
    } finally {
      loading.value = false
    }
  }, 300)
}

onMounted(() => {
  fetchRecipes()
})
</script>

<style scoped>
.recipes-container {
  max-width: 1200px;
  margin: 0 auto;
}

.recipes-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.search-bar input {
  padding: 0.75rem;
  border: 1px solid #ddd;
  border-radius: 4px;
  width: 300px;
  font-size: 1rem;
}

.search-bar input:focus {
  outline: none;
  border-color: #4CAF50;
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
}

.recipe-card:hover {
  transform: translateY(-4px);
}

.recipe-image {
  width: 100%;
  height: 200px;
  object-fit: cover;
}

.recipe-content {
  padding: 1.5rem;
}

.recipe-title {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.recipe-description {
  color: #666;
  margin-bottom: 1rem;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.recipe-meta {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  color: #666;
  font-size: 0.9rem;
  flex-wrap: wrap;
}

.recipe-tags {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
  flex-wrap: wrap;
}

.tag {
  padding: 0.25rem 0.5rem;
  border-radius: 12px;
  font-size: 0.8rem;
  color: white;
  font-weight: 500;
}

.view-recipe {
  display: inline-block;
  padding: 0.5rem 1rem;
  background-color: #4CAF50;
  color: white;
  text-decoration: none;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.view-recipe:hover {
  background-color: #45a049;
}

.loading, .no-recipes {
  text-align: center;
  padding: 2rem;
  color: #666;
}
</style> 