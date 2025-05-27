<template>
  <div class="categories-view">
    <div class="header">
      <h1>Categories Management</h1>
      <button @click="showCreateModal = true" class="btn btn-primary">
        <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
          <path d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"/>
        </svg>
        Add Category
      </button>
    </div>

    <div class="categories-grid" v-if="categories.length > 0">
      <div v-for="category in categories" :key="category.id" class="category-card">
        <div class="category-content">
          <h3>{{ category.name }}</h3>
          <p v-if="category.description">{{ category.description }}</p>
          <p v-else class="no-description">No description</p>
        </div>
        <div class="category-actions">
          <button @click="editCategory(category)" class="btn-icon" title="Edit">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708L10.5 9.207l-3-3L12.146.146zM11.207 9.5L7 13.707V10.5a.5.5 0 0 0-.5-.5H3.207L11.207 9.5z"/>
            </svg>
          </button>
          <button @click="deleteCategory(category)" class="btn-icon btn-danger" title="Delete">
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
        <path d="M9.828 3h3.982a2 2 0 0 1 1.992 2.181l-.637 7A2 2 0 0 1 13.174 14H2.825a2 2 0 0 1-1.991-1.819l-.637-7a1.99 1.99 0 0 1 .342-1.31L.5 3a2 2 0 0 1 2-2h3.672a2 2 0 0 1 1.414.586l.828.828A2 2 0 0 0 9.828 3zm-8.322.12C1.72 3.042 1.95 3 2.19 3h5.396l-.707-.707A1 1 0 0 0 6.172 2H2.5a1 1 0 0 0-1 .981l.006.139z"/>
      </svg>
      <h3>No categories yet</h3>
      <p>Create your first category to get started</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="loading"></div>
      <p>Loading categories...</p>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ showEditModal ? 'Edit Category' : 'Create Category' }}</h2>
          <button @click="closeModal" class="btn-close">×</button>
        </div>
        <form @submit.prevent="saveCategory" class="modal-body">
          <div class="form-group">
            <label class="form-label">Name *</label>
            <input 
              v-model="categoryForm.name" 
              type="text" 
              class="form-input" 
              required 
              maxlength="100"
              placeholder="Enter category name"
            />
          </div>
          <div class="form-group">
            <label class="form-label">Description</label>
            <textarea 
              v-model="categoryForm.description" 
              class="form-input" 
              rows="3"
              maxlength="500"
              placeholder="Enter category description (optional)"
            ></textarea>
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
import { categoriesAPI } from '@/services/api'

const categories = ref([])
const loading = ref(true)
const saving = ref(false)
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingCategory = ref(null)

const categoryForm = ref({
  name: '',
  description: ''
})

const loadCategories = async () => {
  try {
    loading.value = true
    const response = await categoriesAPI.getAll()
    categories.value = response.data
  } catch (error) {
    console.error('Error loading categories:', error)
    alert('Failed to load categories')
  } finally {
    loading.value = false
  }
}

const editCategory = (category) => {
  editingCategory.value = category
  categoryForm.value = {
    name: category.name,
    description: category.description || ''
  }
  showEditModal.value = true
}

const deleteCategory = async (category) => {
  if (!confirm(`Are you sure you want to delete "${category.name}"?`)) {
    return
  }

  try {
    await categoriesAPI.delete(category.id)
    categories.value = categories.value.filter(c => c.id !== category.id)
  } catch (error) {
    console.error('Error deleting category:', error)
    if (error.response?.status === 400) {
      alert('Cannot delete category that is being used by recipes')
    } else {
      alert('Failed to delete category')
    }
  }
}

const saveCategory = async () => {
  try {
    saving.value = true
    
    if (showEditModal.value) {
      const response = await categoriesAPI.update(editingCategory.value.id, categoryForm.value)
      const index = categories.value.findIndex(c => c.id === editingCategory.value.id)
      categories.value[index] = response.data
    } else {
      const response = await categoriesAPI.create(categoryForm.value)
      categories.value.push(response.data)
    }
    
    closeModal()
  } catch (error) {
    console.error('Error saving category:', error)
    alert('Failed to save category')
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingCategory.value = null
  categoryForm.value = {
    name: '',
    description: ''
  }
}

onMounted(() => {
  loadCategories()
})
</script>

<style scoped>
.categories-view {
  max-width: 1200px;
  margin: 0 auto;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.header h1 {
  color: #333;
  font-size: 2rem;
  font-weight: 600;
}

.categories-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.category-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.category-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.category-content h3 {
  color: #333;
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.category-content p {
  color: #666;
  line-height: 1.5;
  margin-bottom: 1rem;
}

.no-description {
  font-style: italic;
  color: #999;
}

.category-actions {
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

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}

textarea.form-input {
  resize: vertical;
  min-height: 80px;
}
</style> 