<template>
  <div class="ingredients-view">
    <div class="header">
      <h1>Ingredients Management</h1>
      <div class="header-actions">
        <div class="search-box">
          <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor" class="search-icon">
            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z"/>
          </svg>
          <input 
            v-model="searchTerm" 
            @input="searchIngredients"
            type="text" 
            placeholder="Search ingredients..."
            class="search-input"
          />
        </div>
        <button @click="showCreateModal = true" class="btn btn-primary">
          <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
            <path d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"/>
          </svg>
          Add Ingredient
        </button>
      </div>
    </div>

    <div class="ingredients-grid" v-if="ingredients.length > 0">
      <div v-for="ingredient in ingredients" :key="ingredient.id" class="ingredient-card">
        <div class="ingredient-content">
          <h3>{{ ingredient.name }}</h3>
          <p v-if="ingredient.unit" class="unit">Unit: {{ ingredient.unit }}</p>
          <p v-else class="no-unit">No unit specified</p>
        </div>
        <div class="ingredient-actions">
          <button @click="editIngredient(ingredient)" class="btn-icon" title="Edit">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708L10.5 9.207l-3-3L12.146.146zM11.207 9.5L7 13.707V10.5a.5.5 0 0 0-.5-.5H3.207L11.207 9.5z"/>
            </svg>
          </button>
          <button @click="deleteIngredient(ingredient)" class="btn-icon btn-danger" title="Delete">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0v-6a.5.5 0 0 1 .5-.5zm2.5 0a.5.5 0 0 1 .5.5v6a.5.5 0 0 1-1 0V6a.5.5 0 0 1 .5-.5zm3 .5a.5.5 0 0 0-1 0v6a.5.5 0 0 0 1 0V6z"/>
              <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>
            </svg>
          </button>
        </div>
      </div>
    </div>

    <div v-else-if="!loading" class="empty-state">
      <svg width="64" height="64" viewBox="0 0 16 16" fill="currentColor" class="empty-icon">
        <path d="M8 1a2.5 2.5 0 0 1 2.5 2.5V4h-5v-.5A2.5 2.5 0 0 1 8 1zm3.5 3v-.5a3.5 3.5 0 1 0-7 0V4H1v10a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V4h-3.5zM2 5h12v9a1 1 0 0 1-1 1H3a1 1 0 0 1-1-1V5z"/>
      </svg>
      <h3>{{ searchTerm ? 'No ingredients found' : 'No ingredients yet' }}</h3>
      <p>{{ searchTerm ? 'Try adjusting your search terms' : 'Create your first ingredient to get started' }}</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="loading"></div>
      <p>Loading ingredients...</p>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ showEditModal ? 'Edit Ingredient' : 'Create Ingredient' }}</h2>
          <button @click="closeModal" class="btn-close">×</button>
        </div>
        <form @submit.prevent="saveIngredient" class="modal-body">
          <div class="form-group">
            <label class="form-label">Name *</label>
            <input 
              v-model="ingredientForm.name" 
              type="text" 
              class="form-input" 
              required 
              maxlength="100"
              placeholder="Enter ingredient name"
            />
          </div>
          <div class="form-group">
            <label class="form-label">Unit</label>
            <select v-model="ingredientForm.unit" class="form-input">
              <option value="">Select a unit (optional)</option>
              <option v-for="unit in commonUnits" :key="unit" :value="unit">{{ unit }}</option>
            </select>
            <small class="form-help">Or type a custom unit below</small>
            <input 
              v-model="customUnit" 
              type="text" 
              class="form-input" 
              maxlength="20"
              placeholder="Custom unit (e.g., pinch, dash)"
              @input="ingredientForm.unit = customUnit"
            />
          </div>
          <div class="modal-actions">
            <button type="button" @click="closeModal" class="btn btn-secondary">Cancel</button>
            <button type="submit" class="btn btn-primary" :disabled="saving">
              <div v-if="saving" class="loading"></div>
              {{ saving ? 'Saving...' : (showEditModal ? 'Update' : 'Create') }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { ingredientsAPI } from '@/services/api'

const ingredients = ref([])
const loading = ref(true)
const saving = ref(false)
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingIngredient = ref(null)
const searchTerm = ref('')
const customUnit = ref('')

const ingredientForm = ref({
  name: '',
  unit: ''
})

const commonUnits = [
  'cup', 'cups', 'tablespoon', 'tablespoons', 'teaspoon', 'teaspoons',
  'gram', 'grams', 'kilogram', 'kilograms', 'pound', 'pounds',
  'ounce', 'ounces', 'liter', 'liters', 'milliliter', 'milliliters',
  'piece', 'pieces', 'slice', 'slices', 'clove', 'cloves'
]

let searchTimeout = null

const loadIngredients = async (search = '') => {
  try {
    loading.value = true
    const response = await ingredientsAPI.getAll(search)
    ingredients.value = response.data
  } catch (error) {
    console.error('Error loading ingredients:', error)
    alert('Failed to load ingredients')
  } finally {
    loading.value = false
  }
}

const searchIngredients = () => {
  clearTimeout(searchTimeout)
  searchTimeout = setTimeout(() => {
    loadIngredients(searchTerm.value)
  }, 300)
}

const editIngredient = (ingredient) => {
  editingIngredient.value = ingredient
  ingredientForm.value = {
    name: ingredient.name,
    unit: ingredient.unit || ''
  }
  customUnit.value = ingredient.unit || ''
  showEditModal.value = true
}

const deleteIngredient = async (ingredient) => {
  if (!confirm(`Are you sure you want to delete "${ingredient.name}"?`)) {
    return
  }

  try {
    await ingredientsAPI.delete(ingredient.id)
    ingredients.value = ingredients.value.filter(i => i.id !== ingredient.id)
  } catch (error) {
    console.error('Error deleting ingredient:', error)
    if (error.response?.status === 400) {
      alert('Cannot delete ingredient that is being used by recipes')
    } else {
      alert('Failed to delete ingredient')
    }
  }
}

const saveIngredient = async () => {
  try {
    saving.value = true
    
    if (showEditModal.value) {
      const response = await ingredientsAPI.update(editingIngredient.value.id, ingredientForm.value)
      const index = ingredients.value.findIndex(i => i.id === editingIngredient.value.id)
      ingredients.value[index] = response.data
    } else {
      const response = await ingredientsAPI.create(ingredientForm.value)
      ingredients.value.push(response.data)
    }
    
    closeModal()
  } catch (error) {
    console.error('Error saving ingredient:', error)
    if (error.response?.status === 400 && error.response?.data?.message?.includes('already exists')) {
      alert('An ingredient with this name already exists')
    } else {
      alert('Failed to save ingredient')
    }
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingIngredient.value = null
  customUnit.value = ''
  ingredientForm.value = {
    name: '',
    unit: ''
  }
}

onMounted(() => {
  loadIngredients()
})
</script>

<style scoped>
.ingredients-view {
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  gap: 2rem;
}

.header h1 {
  color: #333;
  font-size: 2rem;
  font-weight: 600;
}

.header-actions {
  display: flex;
  gap: 1rem;
  align-items: center;
}

.search-box {
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 0.75rem;
  color: #666;
  z-index: 1;
}

.search-input {
  padding: 0.75rem 0.75rem 0.75rem 2.5rem;
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  font-size: 1rem;
  width: 300px;
  transition: border-color 0.2s ease;
}

.search-input:focus {
  outline: none;
  border-color: #4CAF50;
}

.ingredients-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.ingredient-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.ingredient-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.ingredient-content h3 {
  color: #333;
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.ingredient-content p {
  color: #666;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.unit {
  font-weight: 500;
  color: #4CAF50;
}

.no-unit {
  font-style: italic;
  color: #999;
}

.ingredient-actions {
  display: flex;
  gap: 0.5rem;
  justify-content: flex-end;
}

.btn-icon {
  padding: 0.5rem;
  border: none;
  border-radius: 6px;
  background-color: #f8f9fa;
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

.empty-state, .loading-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #666;
}

.empty-icon {
  color: #ccc;
  margin-bottom: 1rem;
}

.empty-state h3 {
  color: #333;
  margin-bottom: 0.5rem;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal {
  background: white;
  border-radius: 12px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #e9ecef;
}

.modal-header h2 {
  color: #333;
  font-size: 1.5rem;
  font-weight: 600;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #666;
  cursor: pointer;
  padding: 0.25rem;
  line-height: 1;
}

.btn-close:hover {
  color: #333;
}

.modal-body {
  padding: 1.5rem;
}

.form-help {
  display: block;
  margin-top: 0.25rem;
  margin-bottom: 0.5rem;
  color: #666;
  font-size: 0.875rem;
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}

@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }
  
  .header-actions {
    flex-direction: column;
  }
  
  .search-input {
    width: 100%;
  }
}
</style> 