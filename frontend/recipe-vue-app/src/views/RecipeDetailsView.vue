<template>
  <div class="details-container" v-if="recipe">
    <button class="back-btn" @click="goBack">&larr; Back</button>
    <div class="details-card">
      <img v-if="recipe.imageUrl" :src="recipe.imageUrl" :alt="recipe.title" class="details-image" />
      <div class="details-content">
        <h1 class="details-title">{{ recipe.title }}</h1>
        <p class="details-description">{{ recipe.description }}</p>
        <div class="details-meta">
          <span><i class="fas fa-clock"></i> {{ recipe.cookTimeMinutes || 0 }} mins</span>
          <span><i class="fas fa-signal"></i> {{ recipe.difficulty }}</span>
          <span><i class="fas fa-user"></i> {{ recipe.servings || 1 }} servings</span>
        </div>
        <div class="details-tags" v-if="recipe.tags && recipe.tags.length">
          <span v-for="tag in recipe.tags" :key="tag.id" class="tag" :style="{ backgroundColor: tag.color }">
            {{ tag.name }}
          </span>
        </div>
        <h2>Ingredients</h2>
        <ul class="ingredients-list">
          <li v-for="ingredient in recipe.ingredients" :key="ingredient.id">
            {{ ingredient.quantity }} {{ ingredient.unit }} {{ ingredient.name }} <span v-if="ingredient.notes">({{ ingredient.notes }})</span>
          </li>
        </ul>
        <h2>Instructions</h2>
        <div class="instructions" v-html="recipe.instructions.replace(/\n/g, '<br>')"></div>
        <div class="details-ratings" v-if="recipe.ratings && recipe.ratings.length">
          <h2>Ratings & Reviews</h2>
          <div v-for="rating in recipe.ratings" :key="rating.id" class="rating-item">
            <span class="rating-stars">{{ '★'.repeat(rating.rating) }}{{ '☆'.repeat(5 - rating.rating) }}</span>
            <span class="rating-user">by {{ rating.user?.username || 'Anonymous' }}</span>
            <p class="rating-review">{{ rating.review }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
  <div v-else class="loading">Loading recipe...</div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'

const route = useRoute()
const router = useRouter()
const recipe = ref(null)

const fetchRecipe = async () => {
  try {
    const response = await api.get(`/recipes/${route.params.id}`)
    recipe.value = response.data
  } catch (error) {
    recipe.value = null
  }
}

const goBack = () => {
  router.back()
}

onMounted(() => {
  fetchRecipe()
})
</script>

<style scoped>
.details-container {
  max-width: 900px;
  margin: 2rem auto;
  padding: 1rem;
}

.back-btn {
  background: none;
  border: none;
  color: #4CAF50;
  font-size: 1rem;
  cursor: pointer;
  margin-bottom: 1rem;
}

.details-card {
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  overflow: hidden;
}

.details-image {
  width: 100%;
  max-height: 350px;
  object-fit: cover;
}

.details-content {
  padding: 2rem;
}

.details-title {
  margin: 0 0 0.5rem 0;
  color: #333;
}

.details-description {
  color: #666;
  margin-bottom: 1rem;
}

.details-meta {
  display: flex;
  gap: 2rem;
  color: #888;
  margin-bottom: 1rem;
  font-size: 1rem;
}

.details-tags {
  margin-bottom: 1rem;
}

.tag {
  display: inline-block;
  color: #fff;
  border-radius: 4px;
  padding: 0.2rem 0.7rem;
  margin-right: 0.5rem;
  font-size: 0.9rem;
}

.ingredients-list {
  margin-bottom: 1.5rem;
  padding-left: 1.2rem;
}

.instructions {
  background: #f9f9f9;
  padding: 1rem;
  border-radius: 6px;
  margin-bottom: 2rem;
  white-space: pre-line;
}

.details-ratings {
  margin-top: 2rem;
}

.rating-item {
  background: #f5f5f5;
  border-radius: 4px;
  padding: 0.7rem 1rem;
  margin-bottom: 1rem;
}

.rating-stars {
  color: #FFD700;
  font-size: 1.1rem;
  margin-right: 0.5rem;
}

.rating-user {
  color: #4CAF50;
  font-size: 0.95rem;
  margin-left: 0.5rem;
}

.rating-review {
  margin: 0.5rem 0 0 0;
  color: #444;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: #666;
}
</style> 