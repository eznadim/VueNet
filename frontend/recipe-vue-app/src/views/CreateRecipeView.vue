<template>
  <div class="create-recipe-container">
    <div class="header-section">
      <div class="header-content">
        <div class="header-icon">👨‍🍳</div>
        <h1>Create New Recipe</h1>
        <p class="header-subtitle">Share your culinary masterpiece with the world</p>
      </div>
    </div>

    <form class="recipe-form" @submit.prevent="submitRecipe">
      <!-- Basic Information Section -->
      <div class="form-section">
        <h2 class="section-title">
          <span class="section-icon">📝</span>
          Basic Information
        </h2>
        
        <div class="form-group">
          <label for="title" class="form-label">
            <span class="label-icon">🍽️</span>
            Recipe Title *
          </label>
          <input 
            id="title" 
            v-model="title" 
            type="text" 
            required 
            class="form-input"
            placeholder="Enter a delicious recipe name..."
          />
        </div>

        <div class="form-group">
          <label for="description" class="form-label">
            <span class="label-icon">📄</span>
            Description
          </label>
          <textarea 
            id="description" 
            v-model="description" 
            rows="3" 
            class="form-input"
            placeholder="Describe your recipe in a few words..."
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="category" class="form-label">
              <span class="label-icon">🏷️</span>
              Category
            </label>
            <select id="category" v-model="categoryId" class="form-input">
              <option value="">Select category</option>
              <option v-for="cat in categories" :key="cat.id" :value="cat.id">
                {{ cat.name }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="difficulty" class="form-label">
              <span class="label-icon">⭐</span>
              Difficulty
            </label>
            <select id="difficulty" v-model="difficulty" class="form-input">
              <option value="Easy">🟢 Easy</option>
              <option value="Medium">🟡 Medium</option>
              <option value="Hard">🔴 Hard</option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label for="imageUrl" class="form-label">
            <span class="label-icon">📸</span>
            Image URL
          </label>
          <input 
            id="imageUrl" 
            v-model="imageUrl" 
            type="url" 
            class="form-input"
            placeholder="https://example.com/your-recipe-image.jpg"
          />
          <div v-if="imageUrl" class="image-preview">
            <img :src="imageUrl" alt="Recipe preview" @error="imageError = true" @load="imageError = false" />
            <p v-if="imageError" class="image-error">⚠️ Unable to load image</p>
          </div>
        </div>
      </div>

      <!-- Timing & Servings Section -->
      <div class="form-section">
        <h2 class="section-title">
          <span class="section-icon">⏱️</span>
          Timing & Servings
        </h2>
        
        <div class="timing-servings-container">
          <div class="timing-inputs">
            <div class="form-group">
              <label for="prepTime" class="form-label">
                <span class="label-icon">🔪</span>
                Prep Time (min)
              </label>
              <input 
                id="prepTime" 
                v-model.number="prepTimeMinutes" 
                type="number" 
                min="0" 
                class="form-input"
                placeholder="15"
              />
            </div>

            <div class="form-group">
              <label for="cookTime" class="form-label">
                <span class="label-icon">🔥</span>
                Cook Time (min)
              </label>
              <input 
                id="cookTime" 
                v-model.number="cookTimeMinutes" 
                type="number" 
                min="0" 
                class="form-input"
                placeholder="30"
              />
            </div>

            <div class="form-group">
              <label for="servings" class="form-label">
                <span class="label-icon">👥</span>
                Servings
              </label>
              <input 
                id="servings" 
                v-model.number="servings" 
                type="number" 
                min="1" 
                class="form-input"
                placeholder="4"
              />
            </div>
          </div>

          <div class="total-time-display">
            <div class="total-time">
              <span class="time-label">Total Time</span>
              <span class="time-value">{{ totalTime }} min</span>
            </div>
          </div>
        </div>
      </div>

      <!-- Ingredients Section -->
      <div class="form-section">
        <h2 class="section-title">
          <span class="section-icon">🥕</span>
          Ingredients
        </h2>
        
        <div class="ingredients-container">
          <div v-for="(ingredient, idx) in ingredients" :key="idx" class="ingredient-row">
            <div class="ingredient-number">{{ idx + 1 }}</div>
            <input 
              v-model="ingredient.quantity" 
              type="number" 
              min="0" 
              step="0.01" 
              placeholder="1" 
              required 
              class="ingredient-input quantity-input"
            />
            <input 
              v-model="ingredient.unit" 
              type="text" 
              placeholder="cup" 
              class="ingredient-input unit-input"
              list="units-list"
            />
            <input 
              v-model="ingredient.name" 
              type="text" 
              placeholder="Ingredient name" 
              required 
              class="ingredient-input name-input"
            />
            <input 
              v-model="ingredient.notes" 
              type="text" 
              placeholder="Optional notes" 
              class="ingredient-input notes-input"
            />
            <button 
              type="button" 
              class="remove-btn" 
              @click="removeIngredient(idx)"
              :disabled="ingredients.length === 1"
            >
              🗑️
            </button>
          </div>
          
          <button type="button" class="add-ingredient-btn" @click="addIngredient">
            <span class="add-icon">➕</span>
            Add Ingredient
          </button>
        </div>

        <!-- Common units datalist -->
        <datalist id="units-list">
          <option value="cup"></option>
          <option value="tbsp"></option>
          <option value="tsp"></option>
          <option value="oz"></option>
          <option value="lb"></option>
          <option value="g"></option>
          <option value="kg"></option>
          <option value="ml"></option>
          <option value="l"></option>
          <option value="piece"></option>
          <option value="clove"></option>
          <option value="slice"></option>
        </datalist>
      </div>

      <!-- Instructions Section -->
      <div class="form-section">
        <h2 class="section-title">
          <span class="section-icon">📋</span>
          Instructions
        </h2>
        
        <div class="form-group">
          <label for="instructions" class="form-label">
            <span class="label-icon">👩‍🍳</span>
            Step-by-step instructions *
          </label>
          <textarea 
            id="instructions" 
            v-model="instructions" 
            rows="8" 
            required 
            class="form-input instructions-textarea"
            placeholder="1. Preheat oven to 350°F...&#10;2. In a large bowl, mix...&#10;3. Bake for 25-30 minutes..."
          />
          <div class="instructions-help">
            💡 Tip: Number your steps and be specific with temperatures and timing
          </div>
        </div>
      </div>

      <!-- Tags Section -->
      <div class="form-section">
        <h2 class="section-title">
          <span class="section-icon">🏷️</span>
          Tags
        </h2>
        
        <div class="tags-container">
          <div v-if="tags.length === 0" class="loading-tags">
            Loading tags...
          </div>
          <div v-else class="tags-grid">
            <label v-for="tag in tags" :key="tag.id" class="tag-checkbox">
              <input type="checkbox" :value="tag.id" v-model="selectedTags" />
              <span class="tag-label" :style="{ backgroundColor: tag.color }">
                {{ tag.name }}
              </span>
            </label>
          </div>
        </div>
      </div>

      <!-- Submit Section -->
      <div class="submit-section">
        <button type="submit" class="submit-btn" :disabled="loading || !isFormValid">
          <span v-if="loading" class="loading-spinner"></span>
          <span class="submit-icon">{{ loading ? '⏳' : '🚀' }}</span>
          {{ loading ? 'Creating Recipe...' : 'Create Recipe' }}
        </button>
        
        <div v-if="success" class="success-message">
          <span class="success-icon">✅</span>
          Recipe created successfully! Redirecting...
        </div>
        
        <div v-if="error" class="error-message">
          <span class="error-icon">❌</span>
          {{ error }}
        </div>
      </div>
    </form>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import api from '@/services/api'
import { useRouter } from 'vue-router'

const router = useRouter()
const title = ref('')
const description = ref('')
const categoryId = ref('')
const imageUrl = ref('')
const imageError = ref(false)
const difficulty = ref('Easy')
const prepTimeMinutes = ref(0)
const cookTimeMinutes = ref(0)
const servings = ref(1)
const ingredients = ref([
  { quantity: '', unit: '', name: '', notes: '' }
])
const instructions = ref('')
const tags = ref([])
const selectedTags = ref([])
const categories = ref([])
const loading = ref(false)
const success = ref(false)
const error = ref('')

const totalTime = computed(() => {
  return (prepTimeMinutes.value || 0) + (cookTimeMinutes.value || 0)
})

const isFormValid = computed(() => {
  return title.value.trim() && 
         instructions.value.trim() && 
         ingredients.value.some(i => i.name.trim() && i.quantity)
})

const fetchTagsAndCategories = async () => {
  try {
    const [tagsRes, catsRes] = await Promise.all([
      api.get('/tags'),
      api.get('/categories')
    ])
    tags.value = tagsRes.data
    categories.value = catsRes.data
  } catch (err) {
    console.error('Failed to fetch tags and categories:', err)
  }
}

const addIngredient = () => {
  ingredients.value.push({ quantity: '', unit: '', name: '', notes: '' })
}

const removeIngredient = (idx) => {
  if (ingredients.value.length > 1) {
    ingredients.value.splice(idx, 1)
  }
}

const submitRecipe = async () => {
  if (!isFormValid.value) {
    error.value = 'Please fill in all required fields.'
    return
  }

  try {
    loading.value = true
    error.value = ''
    success.value = false
    
    const payload = {
      title: title.value.trim(),
      description: description.value.trim(),
      categoryId: categoryId.value ? parseInt(categoryId.value) : null,
      imageUrl: imageUrl.value.trim() || null,
      difficulty: difficulty.value,
      prepTimeMinutes: prepTimeMinutes.value || 0,
      cookTimeMinutes: cookTimeMinutes.value || 0,
      servings: servings.value || 1,
      instructions: instructions.value.trim(),
      ingredients: ingredients.value
        .filter(i => i.name.trim() && i.quantity)
        .map(i => ({
          ingredientName: i.name.trim(),
          quantity: parseFloat(i.quantity) || 0,
          unit: i.unit.trim() || null,
          notes: i.notes.trim() || null
        })),
      tagIds: selectedTags.value
    }
    
    console.log('Sending payload:', payload) // Debug log
    
    await api.post('/recipes', payload)
    success.value = true
    setTimeout(() => router.push('/my-recipes'), 2000)
  } catch (err) {
    console.error('Recipe creation error:', err) // Debug log
    error.value = err.response?.data?.message || 'Failed to create recipe. Please try again.'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchTagsAndCategories()
})
</script>

<style scoped>
.create-recipe-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
  background: #f8f9fa;
  min-height: 100vh;
}

.header-section {
  background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
  border-radius: 20px;
  padding: 3rem 2rem;
  text-align: center;
  margin-bottom: 2rem;
  color: white;
  box-shadow: 0 10px 30px rgba(76, 175, 80, 0.3);
}

.header-icon {
  font-size: 4rem;
  margin-bottom: 1rem;
  filter: drop-shadow(0 4px 8px rgba(0, 0, 0, 0.2));
}

.header-content h1 {
  font-size: 2.5rem;
  margin-bottom: 0.5rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.header-subtitle {
  font-size: 1.1rem;
  opacity: 0.9;
  font-style: italic;
}

.recipe-form {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.form-section {
  background: white;
  border-radius: 16px;
  padding: 2rem;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  border: 1px solid rgba(0, 0, 0, 0.05);
  transition: all 0.3s ease;
}

.form-section:hover {
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
  transform: translateY(-2px);
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.5rem;
  color: #2c3e50;
  margin-bottom: 1.5rem;
  padding-bottom: 0.75rem;
  border-bottom: 2px solid #e9ecef;
}

.section-icon {
  font-size: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin-bottom: 1.5rem;
}

.timing-servings-container {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.timing-inputs {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
}

.form-label {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-weight: 600;
  color: #2c3e50;
  font-size: 1rem;
}

.label-icon {
  font-size: 1.1rem;
}

.form-input {
  padding: 1rem;
  border: 2px solid #e9ecef;
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.3s ease;
  background: #fff;
}

.form-input:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 3px rgba(76, 175, 80, 0.1);
}

.form-input::placeholder {
  color: #6c757d;
}

.image-preview {
  margin-top: 1rem;
  text-align: center;
}

.image-preview img {
  max-width: 300px;
  max-height: 200px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.image-error {
  color: #dc3545;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}

.total-time-display {
  display: flex;
  align-items: center;
  justify-content: center;
}

.total-time {
  background: linear-gradient(135deg, #e3f2fd 0%, #bbdefb 100%);
  padding: 1.5rem 2rem;
  border-radius: 16px;
  text-align: center;
  border: 2px solid #2196f3;
  box-shadow: 0 4px 12px rgba(33, 150, 243, 0.2);
  min-width: 200px;
  transition: all 0.3s ease;
  animation: pulse 2s infinite;
}

.total-time:hover {
  transform: scale(1.05);
  box-shadow: 0 6px 20px rgba(33, 150, 243, 0.3);
}

@keyframes pulse {
  0%, 100% {
    box-shadow: 0 4px 12px rgba(33, 150, 243, 0.2);
  }
  50% {
    box-shadow: 0 4px 12px rgba(33, 150, 243, 0.4);
  }
}

.time-label {
  display: block;
  font-size: 1rem;
  color: #1976d2;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.time-value {
  display: block;
  font-size: 2rem;
  font-weight: 700;
  color: #0d47a1;
}

.ingredients-container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.ingredient-row {
  display: grid;
  grid-template-columns: auto 80px 100px 1fr 150px auto;
  gap: 0.75rem;
  align-items: center;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 12px;
  border: 2px solid #e9ecef;
  transition: all 0.3s ease;
}

.ingredient-row:hover {
  border-color: #4CAF50;
  box-shadow: 0 2px 8px rgba(76, 175, 80, 0.1);
}

.ingredient-number {
  width: 30px;
  height: 30px;
  background: #4CAF50;
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 0.9rem;
}

.ingredient-input {
  padding: 0.75rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  font-size: 0.95rem;
  transition: all 0.3s ease;
}

.ingredient-input:focus {
  outline: none;
  border-color: #4CAF50;
  box-shadow: 0 0 0 2px rgba(76, 175, 80, 0.1);
}

.remove-btn {
  background: #fff5f5;
  color: #dc3545;
  border: 2px solid #f5c6cb;
  border-radius: 8px;
  padding: 0.5rem;
  cursor: pointer;
  font-size: 1.1rem;
  transition: all 0.3s ease;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-btn:hover:not(:disabled) {
  background: #f8d7da;
  border-color: #dc3545;
}

.remove-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.add-ingredient-btn {
  background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);
  color: #2e7d32;
  border: 2px dashed #4CAF50;
  border-radius: 12px;
  padding: 1rem;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.add-ingredient-btn:hover {
  background: linear-gradient(135deg, #c8e6c9 0%, #a5d6a7 100%);
  border-color: #2e7d32;
}

.add-icon {
  font-size: 1.2rem;
}

.instructions-textarea {
  min-height: 200px;
  resize: vertical;
  font-family: inherit;
  line-height: 1.6;
}

.instructions-help {
  background: #e3f2fd;
  color: #1565c0;
  padding: 0.75rem;
  border-radius: 8px;
  font-size: 0.9rem;
  margin-top: 0.5rem;
}

.tags-container {
  margin-top: 1rem;
}

.loading-tags {
  text-align: center;
  color: #6c757d;
  font-style: italic;
}

.tags-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 1rem;
}

.tag-checkbox {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.tag-checkbox:hover {
  background: #f8f9fa;
}

.tag-checkbox input[type="checkbox"] {
  width: 18px;
  height: 18px;
  accent-color: #4CAF50;
}

.tag-label {
  color: white;
  border-radius: 20px;
  padding: 0.4rem 1rem;
  font-size: 0.9rem;
  font-weight: 500;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.submit-section {
  text-align: center;
  padding: 2rem;
}

.submit-btn {
  background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
  color: white;
  padding: 1.25rem 3rem;
  border: none;
  border-radius: 50px;
  font-size: 1.2rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.75rem;
  box-shadow: 0 8px 25px rgba(76, 175, 80, 0.3);
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 12px 35px rgba(76, 175, 80, 0.4);
}

.submit-btn:disabled {
  background: linear-gradient(135deg, #bdc3c7 0%, #95a5a6 100%);
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.submit-icon {
  font-size: 1.3rem;
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  border-top-color: #fff;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.success-message {
  background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
  color: #155724;
  padding: 1rem 2rem;
  border-radius: 12px;
  margin-top: 1rem;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  border: 2px solid #28a745;
}

.error-message {
  background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
  color: #721c24;
  padding: 1rem 2rem;
  border-radius: 12px;
  margin-top: 1rem;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  border: 2px solid #dc3545;
}

.success-icon, .error-icon {
  font-size: 1.2rem;
}

/* Responsive Design */
@media (max-width: 768px) {
  .create-recipe-container {
    padding: 1rem;
  }
  
  .header-section {
    padding: 2rem 1rem;
  }
  
  .header-content h1 {
    font-size: 2rem;
  }
  
  .form-section {
    padding: 1.5rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .timing-servings-container {
    flex-direction: column;
  }
  
  .timing-inputs {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .total-time {
    padding: 1rem 1.5rem;
    min-width: auto;
  }
  
  .time-value {
    font-size: 1.5rem;
  }
  
  .ingredient-row {
    grid-template-columns: auto 1fr auto;
    gap: 0.5rem;
  }
  
  .quantity-input, .unit-input {
    grid-column: 2;
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 0.5rem;
  }
  
  .name-input, .notes-input {
    grid-column: 2;
  }
  
  .tags-grid {
    grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
  }
}

@media (max-width: 480px) {
  .timing-servings-container {
    gap: 1.5rem;
  }
  
  .timing-inputs {
    gap: 1rem;
  }
  
  .ingredient-row {
    grid-template-columns: 1fr;
    text-align: center;
  }
  
  .ingredient-number {
    justify-self: center;
  }
  
  .submit-btn {
    padding: 1rem 2rem;
    font-size: 1rem;
  }
}
</style> 