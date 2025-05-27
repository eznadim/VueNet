<template>
  <div class="recipe-rating">
    <div class="rating-summary">
      <div class="average-rating">
        <div class="stars">
          <span 
            v-for="star in 5" 
            :key="star" 
            class="star"
            :class="{ filled: star <= Math.round(averageRating.averageRating) }"
          >
            ★
          </span>
        </div>
        <div class="rating-info">
          <span class="average">{{ averageRating.averageRating.toFixed(1) }}</span>
          <span class="count">({{ averageRating.totalRatings }} {{ averageRating.totalRatings === 1 ? 'rating' : 'ratings' }})</span>
        </div>
      </div>
      
      <button 
        v-if="!userRating && isAuthenticated" 
        @click="showRatingForm = true" 
        class="btn btn-outline"
      >
        Rate this recipe
      </button>
    </div>

    <!-- User's rating form -->
    <div v-if="showRatingForm" class="rating-form">
      <h4>{{ userRating ? 'Update your rating' : 'Rate this recipe' }}</h4>
      <form @submit.prevent="submitRating">
        <div class="form-group">
          <label class="form-label">Rating *</label>
          <div class="star-input">
            <span 
              v-for="star in 5" 
              :key="star"
              class="star-input-item"
              :class="{ active: star <= ratingForm.rating }"
              @click="ratingForm.rating = star"
              @mouseover="hoverRating = star"
              @mouseleave="hoverRating = 0"
            >
              ★
            </span>
          </div>
        </div>
        <div class="form-group">
          <label class="form-label">Review (optional)</label>
          <textarea 
            v-model="ratingForm.review" 
            class="form-input" 
            rows="3"
            maxlength="1000"
            placeholder="Share your thoughts about this recipe..."
          ></textarea>
        </div>
        <div class="form-actions">
          <button type="button" @click="cancelRating" class="btn btn-secondary">Cancel</button>
          <button type="submit" class="btn btn-primary" :disabled="saving || !ratingForm.rating">
            <div v-if="saving" class="loading"></div>
            {{ saving ? 'Saving...' : (userRating ? 'Update Rating' : 'Submit Rating') }}
          </button>
        </div>
      </form>
    </div>

    <!-- User's existing rating -->
    <div v-if="userRating && !showRatingForm" class="user-rating">
      <h4>Your rating</h4>
      <div class="rating-card">
        <div class="rating-header">
          <div class="stars">
            <span 
              v-for="star in 5" 
              :key="star" 
              class="star"
              :class="{ filled: star <= userRating.rating }"
            >
              ★
            </span>
          </div>
          <div class="rating-actions">
            <button @click="editRating" class="btn-icon" title="Edit">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708L10.5 9.207l-3-3L12.146.146zM11.207 9.5L7 13.707V10.5a.5.5 0 0 0-.5-.5H3.207L11.207 9.5z"/>
              </svg>
            </button>
            <button @click="deleteRating" class="btn-icon btn-danger" title="Delete">
              <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
                <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/>
                <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>
              </svg>
            </button>
          </div>
        </div>
        <p v-if="userRating.review" class="review">{{ userRating.review }}</p>
      </div>
    </div>

    <!-- All ratings -->
    <div v-if="ratings.length > 0" class="all-ratings">
      <h4>Reviews ({{ ratings.length }})</h4>
      <div class="ratings-list">
        <div v-for="rating in ratings" :key="rating.id" class="rating-item">
          <div class="rating-header">
            <div class="user-info">
              <strong>{{ rating.username }}</strong>
              <div class="stars">
                <span 
                  v-for="star in 5" 
                  :key="star" 
                  class="star small"
                  :class="{ filled: star <= rating.rating }"
                >
                  ★
                </span>
              </div>
            </div>
            <span class="rating-date">{{ formatDate(rating.createdAt) }}</span>
          </div>
          <p v-if="rating.review" class="review">{{ rating.review }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { ratingsAPI } from '@/services/api'

const props = defineProps({
  recipeId: {
    type: Number,
    required: true
  }
})

const ratings = ref([])
const averageRating = ref({ averageRating: 0, totalRatings: 0 })
const userRating = ref(null)
const showRatingForm = ref(false)
const saving = ref(false)
const hoverRating = ref(0)

const ratingForm = ref({
  rating: 0,
  review: ''
})

const isAuthenticated = computed(() => !!localStorage.getItem('token'))

const loadRatings = async () => {
  try {
    const [ratingsResponse, averageResponse] = await Promise.all([
      ratingsAPI.getRecipeRatings(props.recipeId),
      ratingsAPI.getRecipeAverage(props.recipeId)
    ])
    
    ratings.value = ratingsResponse.data
    averageRating.value = averageResponse.data
    
    // Find user's rating if authenticated
    if (isAuthenticated.value) {
      const userData = JSON.parse(localStorage.getItem('user') || '{}')
      userRating.value = ratings.value.find(r => r.userId === userData.id) || null
    }
  } catch (error) {
    console.error('Error loading ratings:', error)
  }
}

const submitRating = async () => {
  try {
    saving.value = true
    
    const ratingData = {
      recipeId: props.recipeId,
      rating: ratingForm.value.rating,
      review: ratingForm.value.review || null
    }
    
    if (userRating.value) {
      // Update existing rating
      const response = await ratingsAPI.update(userRating.value.id, {
        rating: ratingForm.value.rating,
        review: ratingForm.value.review || null
      })
      userRating.value = response.data
      
      // Update in ratings list
      const index = ratings.value.findIndex(r => r.id === userRating.value.id)
      if (index !== -1) {
        ratings.value[index] = response.data
      }
    } else {
      // Create new rating
      const response = await ratingsAPI.create(ratingData)
      userRating.value = response.data
      ratings.value.unshift(response.data)
    }
    
    // Reload average
    const averageResponse = await ratingsAPI.getRecipeAverage(props.recipeId)
    averageRating.value = averageResponse.data
    
    showRatingForm.value = false
    resetForm()
  } catch (error) {
    console.error('Error submitting rating:', error)
    if (error.response?.status === 400) {
      alert(error.response.data.message || 'Failed to submit rating')
    } else {
      alert('Failed to submit rating')
    }
  } finally {
    saving.value = false
  }
}

const editRating = () => {
  ratingForm.value = {
    rating: userRating.value.rating,
    review: userRating.value.review || ''
  }
  showRatingForm.value = true
}

const deleteRating = async () => {
  if (!confirm('Are you sure you want to delete your rating?')) {
    return
  }
  
  try {
    await ratingsAPI.delete(userRating.value.id)
    ratings.value = ratings.value.filter(r => r.id !== userRating.value.id)
    userRating.value = null
    
    // Reload average
    const averageResponse = await ratingsAPI.getRecipeAverage(props.recipeId)
    averageRating.value = averageResponse.data
  } catch (error) {
    console.error('Error deleting rating:', error)
    alert('Failed to delete rating')
  }
}

const cancelRating = () => {
  showRatingForm.value = false
  resetForm()
}

const resetForm = () => {
  ratingForm.value = {
    rating: 0,
    review: ''
  }
  hoverRating.value = 0
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

onMounted(() => {
  loadRatings()
})
</script>

<style scoped>
.recipe-rating {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.rating-summary {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.average-rating {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.stars {
  display: flex;
  gap: 0.25rem;
}

.star {
  font-size: 1.5rem;
  color: #ddd;
  transition: color 0.2s ease;
}

.star.small {
  font-size: 1rem;
}

.star.filled {
  color: #ffc107;
}

.rating-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.average {
  font-size: 1.25rem;
  font-weight: 600;
  color: #333;
}

.count {
  font-size: 0.875rem;
  color: #666;
}

.rating-form {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 1.5rem;
}

.rating-form h4 {
  color: #333;
  margin-bottom: 1rem;
}

.star-input {
  display: flex;
  gap: 0.25rem;
  margin-bottom: 0.5rem;
}

.star-input-item {
  font-size: 2rem;
  color: #ddd;
  cursor: pointer;
  transition: color 0.2s ease;
}

.star-input-item:hover,
.star-input-item.active {
  color: #ffc107;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 1rem;
}

.user-rating {
  margin-bottom: 1.5rem;
}

.user-rating h4 {
  color: #333;
  margin-bottom: 1rem;
}

.rating-card {
  background: #f8f9fa;
  border-radius: 8px;
  padding: 1rem;
}

.rating-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.rating-actions {
  display: flex;
  gap: 0.5rem;
}

.btn-icon {
  padding: 0.5rem;
  border: none;
  border-radius: 6px;
  background-color: white;
  color: #666;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-icon:hover {
  background-color: #e9ecef;
  color: #333;
}

.btn-danger:hover {
  background-color: #dc3545;
  color: white;
}

.all-ratings h4 {
  color: #333;
  margin-bottom: 1rem;
}

.ratings-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.rating-item {
  border-bottom: 1px solid #e9ecef;
  padding-bottom: 1rem;
}

.rating-item:last-child {
  border-bottom: none;
  padding-bottom: 0;
}

.rating-item .rating-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 0.5rem;
}

.user-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.rating-date {
  font-size: 0.875rem;
  color: #666;
}

.review {
  color: #666;
  line-height: 1.5;
  margin: 0;
}

@media (max-width: 768px) {
  .rating-summary {
    flex-direction: column;
    align-items: flex-start;
    gap: 1rem;
  }
  
  .rating-item .rating-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.5rem;
  }
}
</style> 