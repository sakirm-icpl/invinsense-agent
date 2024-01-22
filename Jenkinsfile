pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                script {
                    checkout scm
                }
            }
        }

        stage('Build') {
            steps {
                script {
                    bat "dotnet build Invinsense.sln"
                }
            }
        }

        stage('Package MSI') {
            steps {
                script {
                    bat "make IvsAgent"
                    bat "make IvsTray"
                }
            }
        }
    }

    post {
        success {
            archiveArtifacts artifacts: 'path/to/msi/**', allowEmptyArchive: true
        }
    }
}
