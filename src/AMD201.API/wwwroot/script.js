const { createApp } = Vue;
const { createClient } = supabase;

// Replace with your actual Supabase credentials
const SUPABASE_URL = 'https://mpuomfxrhhdlgidjujtt.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im1wdW9tZnhyaGhkbGdpZGp1anR0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3NjM0MzE4NTksImV4cCI6MjA3OTAwNzg1OX0.xkVcNtlRrM1dswV488JU0jbSew-m1cwNoI2RdbBnaF4';
const API_BASE_URL = ''; // Empty string means same origin (current server)

// Initialize Supabase client with error handling
let supabaseClient = null;
try {
    if (SUPABASE_URL && SUPABASE_ANON_KEY && SUPABASE_URL.startsWith('http')) {
        supabaseClient = createClient(SUPABASE_URL, SUPABASE_ANON_KEY);
    }
} catch (err) {
    console.warn('Supabase not configured or invalid credentials:', err.message);
}

createApp({
    data() {
        return {
            user: null,
            longUrl: '',
            customCode: '',
            shortUrl: '',
            qrCodeUrl: '',
            loading: false,
            error: '',
            success: '',
            showAuth: false,
            authEmail: '',
            authPassword: '',
            authMode: 'signin',
            activeTab: 'shorten',
            history: [],
            historyLoading: false,
            isChecking: false,
            isAvailable: false,
            baseUrl: window.location.origin,
            editingUrl: null,
            editMode: false,
            editOriginalUrl: '',
            editCustomCode: '',
            showQrModal: false,
            currentQrCode: '',
            currentShortUrl: '',
            toasts: []
        };
    },
    watch: {
        error(newVal) {
            if (newVal) {
                setTimeout(() => {
                    this.error = '';
                }, 5000);
            }
        },
        success(newVal) {
            if (newVal) {
                setTimeout(() => {
                    this.success = '';
                }, 5000);
            }
        },
        showAuth(newVal) {
            if (newVal) {
                document.body.classList.add('modal-open');
            } else {
                document.body.classList.remove('modal-open');
            }
        }
    },
    async mounted() {
        // Check for existing session only if Supabase is configured
        if (supabaseClient) {
            try {
                const { data: { session } } = await supabaseClient.auth.getSession();
                if (session) {
                    this.user = session.user;
                }

                // Listen for auth changes
                supabaseClient.auth.onAuthStateChange((event, session) => {
                    this.user = session?.user || null;
                });
            } catch (err) {
                console.warn('Auth check failed:', err.message);
            }
        }
    },
    beforeUnmount() {
        document.body.classList.remove('modal-open');
    },
    methods: {
        showToast(message, type = 'success', duration = 3000) {
            const toastId = Date.now();
            const toast = { message, type, id: toastId };
            this.toasts.push(toast);

            setTimeout(() => {
                const index = this.toasts.findIndex(t => t.id === toastId);
                if (index > -1) {
                    this.toasts.splice(index, 1);
                }
            }, duration);
        },

        async shortenUrl() {
            if (!this.longUrl) {
                this.showToast('Please enter a URL', 'error');
                return;
            }

            this.loading = true;
            this.error = '';
            this.success = '';

            try {
                const headers = {
                    'Content-Type': 'application/json'
                };

                // Add auth token if user is signed in
                if (this.user && supabaseClient) {
                    try {
                        const { data: { session } } = await supabaseClient.auth.getSession();
                        if (session) {
                            headers['Authorization'] = `Bearer ${session.access_token}`;
                        }
                    } catch (err) {
                        console.warn('Failed to get session:', err.message);
                    }
                }

                const response = await fetch(`${API_BASE_URL}/api/url/shorten`, {
                    method: 'POST',
                    headers,
                    body: JSON.stringify({
                        originalUrl: this.longUrl,
                        customCode: this.customCode || null
                    })
                });

                const data = await response.json();

                if (response.ok) {
                    this.shortUrl = data.shortUrl;
                    this.qrCodeUrl = data.qrCodeUrl;
                    this.showToast('URL shortened successfully! 🎉', 'success');
                    this.longUrl = '';
                    this.customCode = '';
                } else {
                    this.showToast(data.error || 'Failed to shorten URL', 'error');
                }
            } catch (err) {
                this.showToast('Network error. Please try again.', 'error');
                console.error(err);
            } finally {
                this.loading = false;
            }
        },

        async checkAvailability() {
            if (!this.customCode || this.customCode.length < 3) {
                this.isAvailable = false;
                return;
            }

            this.isChecking = true;
            try {
                const response = await fetch(`${API_BASE_URL}/api/url/check/${this.customCode}`);
                const data = await response.json();
                this.isAvailable = data.available;
            } catch (err) {
                console.error(err);
            } finally {
                this.isChecking = false;
            }
        },

        async loadHistory() {
            if (!this.user || !supabaseClient) return;

            this.historyLoading = true;
            try {
                const { data: { session } } = await supabaseClient.auth.getSession();
                const response = await fetch(`${API_BASE_URL}/api/url/history`, {
                    headers: {
                        'Authorization': `Bearer ${session.access_token}`
                    }
                });

                const data = await response.json();
                this.history = data.urls || [];
            } catch (err) {
                this.error = 'Failed to load history';
                console.error(err);
            } finally {
                this.historyLoading = false;
            }
        },

        copyToClipboard() {
            navigator.clipboard.writeText(this.shortUrl);
            this.showToast('Copied to clipboard! 📋', 'success', 2000);
        },

        showAuthModal() {
            this.showAuth = true;
            this.authMode = 'signin';
            this.error = '';
            document.body.classList.add('modal-open');
        },

        toggleAuthMode() {
            this.authMode = this.authMode === 'signin' ? 'signup' : 'signin';
            this.error = '';
        },

        async signInWithDiscord() {
            if (!supabaseClient) {
                this.error = 'Authentication not configured';
                return;
            }
            
            try {
                const { error } = await supabaseClient.auth.signInWithOAuth({
                    provider: 'discord',
                    options: {
                        redirectTo: window.location.origin
                    }
                });

                if (error) throw error;
            } catch (err) {
                this.error = err.message;
            }
        },

        async signInWithGitHub() {
            if (!supabaseClient) {
                this.error = 'Authentication not configured';
                return;
            }
            
            try {
                const { error } = await supabaseClient.auth.signInWithOAuth({
                    provider: 'github',
                    options: {
                        redirectTo: window.location.origin
                    }
                });

                if (error) throw error;
            } catch (err) {
                this.error = err.message;
            }
        },

        async signIn() {
            if (!supabaseClient) {
                this.showToast('Authentication not configured', 'error');
                return;
            }

            if (!this.authEmail || !this.authPassword) {
                this.showToast('Please enter email and password', 'error');
                return;
            }

            // Trim whitespace
            const email = this.authEmail.trim();
            const password = this.authPassword.trim();

            if (!email || !password) {
                this.showToast('Email and password cannot be empty', 'error');
                return;
            }

            // Basic email validation
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                this.showToast('Please enter a valid email address', 'error');
                return;
            }
            
            try {
                this.loading = true;
                const { data, error } = await supabaseClient.auth.signInWithPassword({
                    email: email,
                    password: password
                });

                if (error) {
                    console.error('Sign in error:', error);
                    throw error;
                }
                
                if (data?.user) {
                    this.showAuth = false;
                    document.body.classList.remove('modal-open');
                    this.showToast('Welcome back! Signed in successfully! 🎉', 'success');
                    this.authEmail = '';
                    this.authPassword = '';
                }
            } catch (err) {
                console.error('Sign in failed:', err);
                // Provide user-friendly error messages
                if (err.message.includes('Invalid login credentials')) {
                    this.showToast('Invalid email or password ❌', 'error');
                } else if (err.message.includes('Email not confirmed')) {
                    this.showToast('Please verify your email first ✉️', 'error');
                } else {
                    this.showToast(err.message || 'Sign in failed. Please try again.', 'error');
                }
            } finally {
                this.loading = false;
            }
        },

        async signUp() {
            if (!supabaseClient) {
                this.showToast('Authentication not configured', 'error');
                return;
            }

            if (!this.authEmail || !this.authPassword) {
                this.showToast('Please enter email and password', 'error');
                return;
            }

            // Trim whitespace
            const email = this.authEmail.trim();
            const password = this.authPassword.trim();

            if (!email || !password) {
                this.showToast('Email and password cannot be empty', 'error');
                return;
            }

            // Basic email validation
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                this.showToast('Please enter a valid email address', 'error');
                return;
            }

            if (password.length < 6) {
                this.showToast('Password must be at least 6 characters', 'error');
                return;
            }
            
            try {
                this.loading = true;
                const { data, error } = await supabaseClient.auth.signUp({
                    email: email,
                    password: password,
                    options: {
                        emailRedirectTo: window.location.origin,
                        data: {
                            email_confirm: false
                        }
                    }
                });

                if (error) {
                    console.error('Sign up error:', error);
                    throw error;
                }

                if (data?.user) {
                    // Check if email confirmation is required
                    if (data.user.identities && data.user.identities.length === 0) {
                        this.showToast('This email is already registered. Please sign in instead.', 'error');
                        setTimeout(() => {
                            this.authMode = 'signin';
                        }, 2000);
                    } else {
                        this.showToast('Account created successfully! 🎉 You can now sign in.', 'success');
                        this.authEmail = '';
                        this.authPassword = '';
                        setTimeout(() => {
                            this.authMode = 'signin';
                        }, 2000);
                    }
                }
            } catch (err) {
                console.error('Sign up failed:', err);
                // Provide user-friendly error messages
                if (err.message.includes('already registered')) {
                    this.showToast('This email is already registered. Please sign in.', 'error');
                    setTimeout(() => {
                        this.authMode = 'signin';
                    }, 2000);
                } else if (err.message.includes('Password should be')) {
                    this.showToast('Password must be at least 6 characters', 'error');
                } else {
                    this.showToast(err.message || 'Sign up failed. Please try again.', 'error');
                }
            } finally {
                this.loading = false;
            }
        },

        async signOut() {
            if (!supabaseClient) return;
            
            await supabaseClient.auth.signOut();
            this.showToast('Signed out successfully! 👋', 'success');
            this.activeTab = 'shorten';
        },

        startEdit(item) {
            this.editingUrl = item;
            this.editMode = true;
            this.editOriginalUrl = item.originalUrl;
            this.editCustomCode = item.isCustom ? item.shortCode : '';
            this.activeTab = 'shorten';
            window.scrollTo(0, 0);
        },

        cancelEdit() {
            this.editMode = false;
            this.editingUrl = null;
            this.editOriginalUrl = '';
            this.editCustomCode = '';
        },

        viewQrCode(item) {
            this.currentQrCode = item.qrCodeUrl;
            this.currentShortUrl = `${this.baseUrl}/${item.shortCode}`;
            this.showQrModal = true;
        },

        async saveEdit() {
            if (!this.editingUrl) return;

            this.loading = true;
            this.error = '';
            this.success = '';

            try {
                const headers = {
                    'Content-Type': 'application/json'
                };

                if (this.user && supabaseClient) {
                    try {
                        const { data: { session } } = await supabaseClient.auth.getSession();
                        if (session) {
                            headers['Authorization'] = `Bearer ${session.access_token}`;
                        }
                    } catch (err) {
                        console.warn('Failed to get session:', err.message);
                    }
                }

                const body = {
                    originalUrl: this.editOriginalUrl || null,
                    generateRandom: !this.editCustomCode || this.editCustomCode.trim() === ''
                };

                if (this.editCustomCode && this.editCustomCode.trim() !== '') {
                    body.newCustomCode = this.editCustomCode.trim();
                }

                const response = await fetch(`${API_BASE_URL}/api/url/${this.editingUrl.shortCode}`, {
                    method: 'PUT',
                    headers,
                    body: JSON.stringify(body)
                });

                const data = await response.json();

                if (response.ok) {
                    this.showToast('URL updated successfully! 💾', 'success');
                    this.cancelEdit();
                    await this.loadHistory();
                } else {
                    this.showToast(data.error || 'Failed to update URL', 'error');
                }
            } catch (err) {
                this.showToast('Network error. Please try again.', 'error');
                console.error(err);
            } finally {
                this.loading = false;
            }
        },

        async deleteUrl(item) {
            if (!confirm(`Are you sure you want to delete ${this.baseUrl}/${item.shortCode}?`)) {
                return;
            }

            try {
                const headers = {};
                if (this.user && supabaseClient) {
                    const { data: { session } } = await supabaseClient.auth.getSession();
                    if (session) {
                        headers['Authorization'] = `Bearer ${session.access_token}`;
                    }
                }

                const response = await fetch(`${API_BASE_URL}/api/url/${item.shortCode}`, {
                    method: 'DELETE',
                    headers
                });

                if (response.ok) {
                    this.showToast('URL deleted successfully! 🗑️', 'success');
                    await this.loadHistory();
                } else {
                    const data = await response.json();
                    this.showToast(data.error || 'Failed to delete URL', 'error');
                }
            } catch (err) {
                this.showToast('Network error. Please try again.', 'error');
                console.error(err);
            }
        },

        formatDate(date) {
            return new Date(date).toLocaleDateString();
        }
    }
}).mount('#app');
