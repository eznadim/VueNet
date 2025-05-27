<template>
  <div class="tags-view">
    <div class="header">
      <h1>Tags Management</h1>
      <button @click="showCreateModal = true" class="btn btn-primary">
        <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
          <path d="M8 2a.5.5 0 0 1 .5.5v5h5a.5.5 0 0 1 0 1h-5v5a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"/>
        </svg>
        Add Tag
      </button>
    </div>

    <div class="tags-grid" v-if="tags.length > 0">
      <div v-for="tag in tags" :key="tag.id" class="tag-card">
        <div class="tag-content">
          <div class="tag-header">
            <span class="tag-badge" :style="{ backgroundColor: tag.color || '#6c757d' }">
              {{ tag.name }}
            </span>
          </div>
        </div>
        <div class="tag-actions">
          <button @click="editTag(tag)" class="btn-icon" title="Edit">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M12.146.146a.5.5 0 0 1 .708 0l3 3a.5.5 0 0 1 0 .708L10.5 9.207l-3-3L12.146.146zM11.207 9.5L7 13.707V10.5a.5.5 0 0 0-.5-.5H3.207L11.207 9.5z"/>
            </svg>
          </button>
          <button @click="deleteTag(tag)" class="btn-icon btn-danger" title="Delete">
            <svg width="16" height="16" viewBox="0 0 16 16" fill="currentColor">
              <path d="M5.5 5.5A.5.5 0 0 1 6 6v6a.5.5 0 0 1-1 0v-5h-5a.5.5 0 0 1 0-1h5v-5A.5.5 0 0 1 8 2z"/>
              <path fill-rule="evenodd" d="M14.5 3a1 1 0 0 1-1 1H13v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V4h-.5a1 1 0 0 1-1-1V2a1 1 0 0 1 1-1H6a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1h3.5a1 1 0 0 1 1 1v1zM4.118 4 4 4.059V13a1 1 0 0 0 1 1h6a1 1 0 0 0 1-1V4.059L11.882 4H4.118zM2.5 3V2h11v1h-11z"/>
            </svg>
          </button>
        </div>
      </div>
    </div>

    <div v-else-if="!loading" class="empty-state">
      <svg width="64" height="64" viewBox="0 0 16 16" fill="currentColor" class="empty-icon">
        <path d="M2 2a2 2 0 0 1 2-2h8a2 2 0 0 1 2 2v13.5a.5.5 0 0 1-.777.416L8 13.101l-5.223 2.815A.5.5 0 0 1 2 15.5V2zm2-1a1 1 0 0 0-1 1v12.566l4.723-2.482a.5.5 0 0 1 .554 0L13 14.566V2a1 1 0 0 0-1-1H4z"/>
      </svg>
      <h3>No tags yet</h3>
      <p>Create your first tag to get started</p>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="loading"></div>
      <p>Loading tags...</p>
    </div>

    <!-- Create/Edit Modal -->
    <div v-if="showCreateModal || showEditModal" class="modal-overlay" @click="closeModal">
      <div class="modal" @click.stop>
        <div class="modal-header">
          <h2>{{ showEditModal ? 'Edit Tag' : 'Create Tag' }}</h2>
          <button @click="closeModal" class="btn-close">×</button>
        </div>
        <form @submit.prevent="saveTag" class="modal-body">
          <div class="form-group">
            <label class="form-label">Name *</label>
            <input 
              v-model="tagForm.name" 
              type="text" 
              class="form-input" 
              required 
              maxlength="50"
              placeholder="Enter tag name"
            />
          </div>
          <div class="form-group">
            <label class="form-label">Color</label>
            <div class="color-input-group">
              <input 
                v-model="tagForm.color" 
                type="color" 
                class="color-input"
              />
              <input 
                v-model="tagForm.color" 
                type="text" 
                class="form-input color-text"
                placeholder="#6c757d"
                maxlength="7"
              />
            </div>
            <div class="color-preview">
              <span class="tag-badge" :style="{ backgroundColor: tagForm.color || '#6c757d' }">
                {{ tagForm.name || 'Preview' }}
              </span>
            </div>
          </div>
          <div class="color-presets">
            <label class="form-label">Quick Colors</label>
            <div class="preset-colors">
              <button 
                v-for="color in presetColors" 
                :key="color"
                type="button"
                class="preset-color"
                :style="{ backgroundColor: color }"
                @click="tagForm.color = color"
                :class="{ active: tagForm.color === color }"
              ></button>
            </div>
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
import { tagsAPI } from '@/services/api'

const tags = ref([])
const loading = ref(true)
const saving = ref(false)
const showCreateModal = ref(false)
const showEditModal = ref(false)
const editingTag = ref(null)

const tagForm = ref({
  name: '',
  color: '#4CAF50'
})

const presetColors = [
  '#4CAF50', '#2196F3', '#FF9800', '#F44336', '#9C27B0',
  '#607D8B', '#795548', '#E91E63', '#00BCD4', '#8BC34A',
  '#FFC107', '#673AB7', '#3F51B5', '#009688', '#CDDC39'
]

const loadTags = async () => {
  try {
    loading.value = true
    const response = await tagsAPI.getAll()
    tags.value = response.data
  } catch (error) {
    console.error('Error loading tags:', error)
    alert('Failed to load tags')
  } finally {
    loading.value = false
  }
}

const editTag = (tag) => {
  editingTag.value = tag
  tagForm.value = {
    name: tag.name,
    color: tag.color || '#4CAF50'
  }
  showEditModal.value = true
}

const deleteTag = async (tag) => {
  if (!confirm(`Are you sure you want to delete "${tag.name}"?`)) {
    return
  }

  try {
    await tagsAPI.delete(tag.id)
    tags.value = tags.value.filter(t => t.id !== tag.id)
  } catch (error) {
    console.error('Error deleting tag:', error)
    if (error.response?.status === 400) {
      alert('Cannot delete tag that is being used by recipes')
    } else {
      alert('Failed to delete tag')
    }
  }
}

const saveTag = async () => {
  try {
    saving.value = true
    
    if (showEditModal.value) {
      const response = await tagsAPI.update(editingTag.value.id, tagForm.value)
      const index = tags.value.findIndex(t => t.id === editingTag.value.id)
      tags.value[index] = response.data
    } else {
      const response = await tagsAPI.create(tagForm.value)
      tags.value.push(response.data)
    }
    
    closeModal()
  } catch (error) {
    console.error('Error saving tag:', error)
    alert('Failed to save tag')
  } finally {
    saving.value = false
  }
}

const closeModal = () => {
  showCreateModal.value = false
  showEditModal.value = false
  editingTag.value = null
  tagForm.value = {
    name: '',
    color: '#4CAF50'
  }
}

onMounted(() => {
  loadTags()
})
</script>

<style scoped>
.tags-view {
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

.tags-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1.5rem;
}

.tag-card {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.tag-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.tag-badge {
  display: inline-block;
  padding: 0.5rem 1rem;
  border-radius: 20px;
  color: white;
  font-size: 0.9rem;
  font-weight: 500;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.2);
}

.tag-actions {
  display: flex;
  gap: 0.5rem;
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

.color-input-group {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.color-input {
  width: 50px;
  height: 40px;
  border: 2px solid #e1e5e9;
  border-radius: 8px;
  cursor: pointer;
}

.color-text {
  flex: 1;
}

.color-preview {
  margin-top: 1rem;
}

.color-presets {
  margin-top: 1rem;
}

.preset-colors {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.preset-color {
  width: 40px;
  height: 40px;
  border: 2px solid transparent;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.preset-color:hover {
  transform: scale(1.1);
}

.preset-color.active {
  border-color: #333;
  transform: scale(1.1);
}

.modal-actions {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  margin-top: 2rem;
}
</style> 